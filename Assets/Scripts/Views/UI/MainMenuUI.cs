using System;
using System.Collections.Generic;
using AI;
using Networking;
using Pieces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private StyleSheet styleSheet       = null!;
        [Tooltip("For the windows")]
        [SerializeField] private StyleSheet windowStyleSheet = null!;
        [SerializeField] private string     gameScene        = "Game";

        private const int SliderMaxDepth = 3;

        private readonly Dictionary<Piece.Color, Button> colorButtons = new();

        private VisualElement menu        = null!;
        private VisualElement windowLayer = null!;
        private Window        botWindow   = null!;
        private Piece.Color   botColor    = Piece.Color.Black;

        private Window    onlineWindow = null!;
        private Window    hostWindow   = null!;
        private Label     hostCode     = null!;
        private Button    copyButton   = null!;
        private Label     hostStatus   = null!;
        private Window    joinWindow   = null!;
        private TextField codeField    = null!;
        private Button    joinButton   = null!;
        private Label     joinStatus   = null!;

        /// <summary>
        /// The session whose start is being waited for.
        /// </summary>
        private OnlineSession? session;

        private void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            root.styleSheets.Add(windowStyleSheet);
            root.styleSheets.Add(styleSheet);
            root.AddToClassList("game-ui");

            VisualElement background = new();
            background.AddToClassList("main-menu");
            root.Add(background);

            menu = new VisualElement();
            menu.AddToClassList("main-menu__panel");
            background.Add(menu);

            Label title = new("Jress: Revitalized");
            title.AddToClassList("main-menu__title");
            menu.Add(title);

            AddButton(menu, "Play bots",   OpenBotWindow);
            AddButton(menu, "Play local",  PlayLocal);
            AddButton(menu, "Play online", OpenOnlineWindow);
            AddButton(menu, "Settings",    null);
            AddButton(menu, "Exit",        Exit);

            windowLayer = new VisualElement { pickingMode = PickingMode.Ignore };
            windowLayer.AddToClassList("layer");
            root.Add(windowLayer);

            BuildBotWindow();
            BuildOnlineWindows();

            if (MatchSetup.TakeNotice() is string notice) ShowNotice(notice);
        }

        /// <summary>
        /// Opens once the menu has been laid out, so it can go beside it.
        /// </summary>
        private void ShowNotice(string text)
        {
            Window window = new("Game ended");
            window.AddToClassList("online");

            AddHint(window.GetContent(), text);
            AddWindowButton(window.GetContent(), "OK", window.Close);

            void Open(GeometryChangedEvent evt)
            {
                menu.UnregisterCallback<GeometryChangedEvent>(Open);

                OpenBesideMenu(window);
            }

            menu.RegisterCallback<GeometryChangedEvent>(Open);
        }

        private void OnDestroy() => Unwatch();

        /// <param name="onClick">Null disables the button.</param>
        private static void AddButton(VisualElement parent, string text, Action? onClick)
        {
            Button button = new(onClick) { text = text };
            button.AddToClassList("main-menu__button");
            button.SetEnabled(onClick != null);

            parent.Add(button);
        }

        private void BuildBotWindow()
        {
            botWindow = new Window("Play bots");
            botWindow.AddToClassList("bot-select");

            VisualElement content = botWindow.GetContent();

            AddHint(content, "Bot plays");

            VisualElement colors = new();
            colors.AddToClassList("bot-select__colors");
            content.Add(colors);

            foreach (Piece.Color color in new[] { Piece.Color.White, Piece.Color.Black })
            {
                Button button = new(() => SetBotColor(color)) { text = color.ToString() };
                button.AddToClassList("hud-button");
                button.AddToClassList("bot-select__color");
                colors.Add(button);

                colorButtons[color] = button;
            }

            AddHint(content, "Depth");

            VisualElement depthRow = new();
            depthRow.AddToClassList("bot-select__depth");
            content.Add(depthRow);

            SliderInt depthSlider = new(HeuristicBot.MinDepth, SliderMaxDepth)
            {
                value = Mathf.Min(MatchSetup.GetBotDepth(), SliderMaxDepth)
            };
            depthSlider.AddToClassList("bot-select__depth-slider");
            depthRow.Add(depthSlider);

            IntegerField depthField = new() { value = MatchSetup.GetBotDepth() };
            depthField.AddToClassList("bot-select__depth-field");
            depthRow.Add(depthField);

            depthSlider.RegisterValueChangedCallback(change =>
            {
                MatchSetup.SetBotDepth(change.newValue);
                depthField.SetValueWithoutNotify(change.newValue);
            });

            depthField.RegisterValueChangedCallback(change =>
            {
                int depth = Mathf.Max(HeuristicBot.MinDepth, change.newValue);

                MatchSetup.SetBotDepth(depth);
                depthField.SetValueWithoutNotify(depth);
                depthSlider.SetValueWithoutNotify(Mathf.Min(depth, SliderMaxDepth));
            });

            AddHint(content, "Higher depth = a stronger, and laggier, bot");

            AddHint(content, "Bot");

            foreach (Bot bot in BotRegistry.CreateAll())
            {
                Button button = new(() => PlayBot(bot)) { text = bot.GetName() };
                button.AddToClassList("hud-button");
                content.Add(button);
            }

            SetBotColor(botColor);
        }

        private void BuildOnlineWindows()
        {
            onlineWindow = new Window("Play online");
            onlineWindow.AddToClassList("online");

            AddWindowButton(onlineWindow.GetContent(), "Host game", HostGame);
            AddWindowButton(onlineWindow.GetContent(), "Join game", OpenJoinWindow);

            hostWindow = new Window("Host game");
            hostWindow.AddToClassList("online");
            hostWindow.Closed += OnlineSession.LeaveCurrent;

            AddHint(hostWindow.GetContent(), "Code");

            hostCode = new Label();
            hostCode.AddToClassList("online__code");
            hostCode.selection.isSelectable = true;
            hostWindow.GetContent().Add(hostCode);

            copyButton = AddWindowButton(hostWindow.GetContent(), "Copy code", () => GUIUtility.systemCopyBuffer = hostCode.text);

            hostStatus = AddHint(hostWindow.GetContent(), "");

            joinWindow = new Window("Join game");
            joinWindow.AddToClassList("online");
            joinWindow.Closed += OnlineSession.LeaveCurrent;

            AddHint(joinWindow.GetContent(), "Code");

            codeField = new TextField { maxLength = 16 };
            codeField.AddToClassList("online__code-field");
            codeField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode is KeyCode.Return or KeyCode.KeypadEnter) JoinGame();
            }, TrickleDown.TrickleDown);
            joinWindow.GetContent().Add(codeField);

            joinButton = AddWindowButton(joinWindow.GetContent(), "Join", JoinGame);

            joinStatus = AddHint(joinWindow.GetContent(), "");
        }

        private static Button AddWindowButton(VisualElement parent, string text, Action onClick)
        {
            Button button = new(onClick) { text = text };
            button.AddToClassList("hud-button");
            parent.Add(button);

            return button;
        }

        private static Label AddHint(VisualElement parent, string text)
        {
            Label hint = new(text);
            hint.AddToClassList("window__hint");
            parent.Add(hint);

            return hint;
        }

        private void SetBotColor(Piece.Color color)
        {
            botColor = color;

            foreach ((Piece.Color buttonColor, Button button) in colorButtons)
                button.EnableInClassList("hud-button--active", buttonColor == color);
        }

        private void OpenBotWindow() => OpenBesideMenu(botWindow);

        private void OpenBesideMenu(Window window)
        {
            Rect bounds = windowLayer.WorldToLocal(menu.worldBound);

            window.Open(windowLayer, new Vector2(bounds.xMax + 16, bounds.yMin));
        }

        private void OpenOnlineWindow() => OpenBesideMenu(onlineWindow);

        /// <summary>
        /// Swaps the online window for <paramref name="window"/>, leaving any session
        /// the other windows had open.
        /// </summary>
        private void OpenOnlineStep(Window window)
        {
            onlineWindow.Close();
            hostWindow  .Close();
            joinWindow  .Close();

            OpenBesideMenu(window);
        }

        private async void HostGame()
        {
            OpenOnlineStep(hostWindow);

            hostCode.text = "...";
            copyButton.SetEnabled(false);
            hostStatus.text = "Creating game...";

            try
            {
                OnlineSession hosted = await OnlineSession.Host();

                hostCode.text = hosted.GetJoinCode();
                copyButton.SetEnabled(true);
                hostStatus.text = "Waiting for an opponent to join...";

                Watch(hosted);
            }
            catch (OperationCanceledException) {}
            catch (Exception e)
            {
                Debug.LogException(e);

                hostCode.text   = "";
                hostStatus.text = $"Couldn't host: {e.Message}";
            }
        }

        private void OpenJoinWindow()
        {
            OpenOnlineStep(joinWindow);

            joinButton.SetEnabled(true);
            joinStatus.text = "";

            codeField.Focus();
        }

        private async void JoinGame()
        {
            string code = codeField.value.Trim().ToUpperInvariant();

            if (code.Length == 0 || !joinButton.enabledSelf) return;

            joinButton.SetEnabled(false);
            joinStatus.text = "Joining...";

            try
            {
                OnlineSession joined = await OnlineSession.Join(code);

                joinStatus.text = "Connecting...";

                Watch(joined);
            }
            catch (OperationCanceledException) {}
            catch (Exception e)
            {
                Debug.LogException(e);

                joinButton.SetEnabled(true);
                joinStatus.text = $"Couldn't join: {e.Message}";
            }
        }

        private void Watch(OnlineSession watched)
        {
            Unwatch();

            session = watched;
            session.Started      += OnSessionStarted;
            session.Disconnected += OnSessionDisconnected;

            if (session.HasStarted()) OnSessionStarted();
        }

        private void Unwatch()
        {
            if (session == null) return;

            session.Started      -= OnSessionStarted;
            session.Disconnected -= OnSessionDisconnected;
            session = null;
        }

        private void OnSessionStarted()
        {
            if (session == null) return;

            MatchSetup.SetOnline(session);

            Unwatch();

            SceneManager.LoadScene(gameScene);
        }

        private void OnSessionDisconnected(string reason)
        {
            Unwatch();

            OnlineSession.LeaveCurrent();

            hostStatus.text = reason;
            joinStatus.text = reason;
            joinButton.SetEnabled(true);
        }

        private void PlayLocal()
        {
            OnlineSession.LeaveCurrent();
            MatchSetup.SetLocal();

            SceneManager.LoadScene(gameScene);
        }

        private void PlayBot(Bot bot)
        {
            OnlineSession.LeaveCurrent();
            MatchSetup.SetBot(bot, botColor);

            SceneManager.LoadScene(gameScene);
        }

        private static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
