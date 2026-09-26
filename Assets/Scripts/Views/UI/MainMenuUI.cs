using System.Collections.Generic;
using AI;
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

        private readonly Dictionary<Piece.Color, Button> colorButtons = new();

        private VisualElement menu        = null!;
        private VisualElement windowLayer = null!;
        private Window        botWindow   = null!;
        private Piece.Color   botColor    = Piece.Color.Black;

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
            AddButton(menu, "Play online", null);
            AddButton(menu, "Settings",    null);
            AddButton(menu, "Exit",        Exit);

            windowLayer = new VisualElement { pickingMode = PickingMode.Ignore };
            windowLayer.AddToClassList("layer");
            root.Add(windowLayer);

            BuildBotWindow();
        }

        /// <param name="onClick">Null disables the button.</param>
        private static void AddButton(VisualElement parent, string text, System.Action? onClick)
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

            AddHint(content, "Bot");

            foreach (Bot bot in BotRegistry.CreateAll())
            {
                Button button = new(() => PlayBot(bot)) { text = bot.GetName() };
                button.AddToClassList("hud-button");
                content.Add(button);
            }

            SetBotColor(botColor);
        }

        private static void AddHint(VisualElement parent, string text)
        {
            Label hint = new(text);
            hint.AddToClassList("window__hint");
            parent.Add(hint);
        }

        private void SetBotColor(Piece.Color color)
        {
            botColor = color;

            foreach ((Piece.Color buttonColor, Button button) in colorButtons)
                button.EnableInClassList("hud-button--active", buttonColor == color);
        }

        private void OpenBotWindow()
        {
            Rect bounds = windowLayer.WorldToLocal(menu.worldBound);

            botWindow.Open(windowLayer, new Vector2(bounds.xMax + 16, bounds.yMin));
        }

        private void PlayLocal()
        {
            MatchSetup.SetLocal();

            SceneManager.LoadScene(gameScene);
        }

        private void PlayBot(Bot bot)
        {
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
