using System.Collections.Generic;
using Boards;
using Gameplay;
using Gameplay.Decisions;
using Pieces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    /// <summary>
    /// Side panels next to the board, and draggable windows on top of it.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameController     controller = null!;
        [SerializeField] private BoardView          boardView  = null!;
        [SerializeField] private Camera             cam        = null!;
        [SerializeField] private PieceSpriteLibrary pieces     = null!;
        [SerializeField] private IconLibrary        icons      = null!;
        [SerializeField] private StyleSheet         styleSheet = null!;
        [SerializeField] private string             menuScene  = "MainMenu";

        [Tooltip("Gap between the board and the side panels, in panel pixels")]
        [SerializeField] private float panelGap      = 16;
        [SerializeField] private float toastDuration = 3;

        private readonly Dictionary<BoardType, Button>  boardButtons = new();
        private readonly Dictionary<Piece.Color, Label> goldLabels   = new();
        private readonly Dictionary<Piece.Color, VisualElement> goldRows = new();

        private UISprites     sprites     = null!;
        private VisualElement root        = null!;
        private VisualElement leftPanel   = null!;
        private VisualElement rightPanel  = null!;
        private VisualElement windowLayer = null!;
        private VisualElement toastLayer  = null!;
        private Tooltip       tooltip     = null!;
        private Label         turnLabel   = null!;
        private Label         turnCount   = null!;
        private Label         hintLabel   = null!;
        private Button        cancelButton = null!;

        private ShopWindow     shop      = null!;
        private UpgradeWindow  upgrades  = null!;
        private RulesWindow    rules     = null!;
        private DecisionWindow decision  = null!;
        private Window         gameOver  = null!;
        private Label          gameOverLabel = null!;
        private Button         newGameButton = null!;

        private void Start()
        {
            if (cam == null) cam = Camera.main!;

            sprites = new UISprites(pieces, icons);

            root = GetComponent<UIDocument>().rootVisualElement;
            root.styleSheets.Add(styleSheet);
            root.AddToClassList("game-ui");
            root.pickingMode = PickingMode.Ignore;

            windowLayer = CreateLayer("window-layer");
            toastLayer  = CreateLayer("toast-layer");

            tooltip = new Tooltip();
            CreateLayer("tooltip-layer").Add(tooltip);

            BuildLeftPanel();
            BuildRightPanel();
            BuildWindows();

            controller.Changed      += Refresh;
            controller.Announced    += ShowToast;
            controller.OpponentLost += ReturnToMenu;
            controller.SetPointerFilter(IsPointerOverUI);

            Refresh();
        }

        private void OnDestroy()
        {
            if (controller == null) return;

            controller.Changed      -= Refresh;
            controller.Announced    -= ShowToast;
            controller.OpponentLost -= ReturnToMenu;
        }

        private void LateUpdate() => PositionPanels();

        private VisualElement CreateLayer(string className)
        {
            VisualElement layer = new() { pickingMode = PickingMode.Ignore };
            layer.AddToClassList("layer");
            layer.AddToClassList(className);

            root.Add(layer);

            return layer;
        }

        private void BuildLeftPanel()
        {
            leftPanel = CreatePanel("side-panel--left");

            AddHeader(leftPanel, "Game");
            AddButton(leftPanel, "Shop",     () => ToggleWindow(shop,     new Vector2(0, 0)));
            AddButton(leftPanel, "Upgrades", () => ToggleWindow(upgrades, new Vector2(30, 30)));
            AddButton(leftPanel, "Rules",    () => ToggleWindow(rules,    new Vector2(60, 60)));

            AddHeader(leftPanel, "Boards");

            foreach (BoardType type in new[] { BoardType.Normal, BoardType.Heaven, BoardType.Hell })
                boardButtons[type] = AddButton(leftPanel, type.ToString(), () => controller.ShowBoard(type));

            AddHeader(leftPanel, "Menu");
            AddButton(leftPanel, "Quit", () => ReturnToMenu(null));
        }

        /// <param name="notice">Shown in the menu.</param>
        private void ReturnToMenu(string? notice)
        {
            MatchSetup.SetNotice(notice);

            SceneManager.LoadScene(menuScene);
        }

        private void BuildRightPanel()
        {
            rightPanel = CreatePanel("side-panel--right");

            turnCount = new Label();
            turnCount.AddToClassList("turn-count");
            rightPanel.Add(turnCount);

            turnLabel = new Label();
            turnLabel.AddToClassList("turn-label");
            rightPanel.Add(turnLabel);

            AddHeader(rightPanel, "Gold");

            foreach (Piece.Color player in new[] { Piece.Color.White, Piece.Color.Black })
            {
                VisualElement row = new();
                row.AddToClassList("gold-row");

                row.Add(UISprites.CreateImage(sprites.Get("coin"), "gold-row__icon"));

                Label name = new(player.ToString());
                name.AddToClassList("gold-row__name");
                row.Add(name);

                Label amount = new();
                amount.AddToClassList("gold-row__amount");
                row.Add(amount);

                rightPanel.Add(row);

                goldRows  [player] = row;
                goldLabels[player] = amount;
            }

            hintLabel = new Label();
            hintLabel.AddToClassList("hint");
            rightPanel.Add(hintLabel);

            cancelButton = AddButton(rightPanel, "Cancel", controller.CancelMode);
        }

        private void BuildWindows()
        {
            shop     = new ShopWindow(controller, sprites, tooltip);
            upgrades = new UpgradeWindow(controller, sprites, tooltip);
            rules    = new RulesWindow(controller, sprites);
            decision = new DecisionWindow(controller, sprites);

            gameOver = new Window("Game over", false);
            gameOver.AddToClassList("game-over");

            gameOverLabel = new Label();
            gameOverLabel.AddToClassList("game-over__label");
            gameOver.GetContent().Add(gameOverLabel);

            newGameButton = new Button(controller.NewGame) { text = "New game" };
            newGameButton.AddToClassList("hud-button");
            gameOver.GetContent().Add(newGameButton);

            AddButton(gameOver.GetContent(), "Main menu", () => ReturnToMenu(null));
        }

        private VisualElement CreatePanel(string className)
        {
            VisualElement panel = new();
            panel.AddToClassList("side-panel");
            panel.AddToClassList(className);

            root.Insert(0, panel);

            return panel;
        }

        private static void AddHeader(VisualElement parent, string text)
        {
            Label header = new(text);
            header.AddToClassList("panel-header");
            parent.Add(header);
        }

        private static Button AddButton(VisualElement parent, string text, System.Action onClick)
        {
            Button button = new(onClick) { text = text };
            button.AddToClassList("hud-button");
            parent.Add(button);

            return button;
        }

        /// <param name="offset">From the board's top left, if the window isn't open.</param>
        private void ToggleWindow(Window window, Vector2 offset)
        {
            window.Toggle(windowLayer, GetBoardTopLeft() + new Vector2(24, 24) + offset);

            Refresh();
        }

        private void Refresh()
        {
            GameState state = controller.GetState();

            if (state == null || root == null) return;

            Piece.Color turn = state.GetTurn();

            turnCount.text = $"Turn {state.GetTurnNumber() + 1}";
            turnLabel.text = $"{turn} to move";

            turnLabel.EnableInClassList("turn-label--white", turn == Piece.Color.White);
            turnLabel.EnableInClassList("turn-label--black", turn == Piece.Color.Black);

            foreach ((Piece.Color player, Label amount) in goldLabels)
            {
                amount.text = state.GetGold(player).ToString();

                goldRows[player].EnableInClassList("gold-row--current", player == turn);
            }

            foreach ((BoardType type, Button button) in boardButtons)
            {
                button.SetEnabled(state.HasBoard(type));
                button.EnableInClassList("hud-button--active", type == controller.GetViewedBoard());
            }

            hintLabel.text = GetHint(state);
            cancelButton.style.display = controller.GetMode() is GameController.Mode.Buy or GameController.Mode.Upgrade
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (shop    .IsOpen()) shop    .Refresh();
            if (upgrades.IsOpen()) upgrades.Refresh();
            if (rules   .IsOpen()) rules   .Refresh();

            RefreshDecision(state);
            RefreshGameOver(state);
        }

        private string GetHint(GameState state)
        {
            string? item = controller.GetModeItem();

            if (state.IsOver()) return GetResultText(state);

            if (controller.GetBot() is { } bot && !controller.IsLocallyControlled(controller.GetActingPlayer()))
                return $"{bot.GetName()} bot is thinking...";

            if (state.GetDecision() is Decision pending) return $"Waiting for {pending.GetPlayer()}: {pending.GetTitle()}";

            if (controller.IsOnline() && !controller.IsLocallyControlled(state.GetTurn()))
                return $"Waiting for {state.GetTurn()}...";

            return controller.GetMode() switch
            {
                GameController.Mode.Buy     => $"Placing {UISprites.FormatName(item!)}. Click a highlighted square.",
                GameController.Mode.Upgrade => $"Upgrading to {UISprites.FormatName(item!)}. Click a highlighted piece.",
                _                           => ""
            };
        }

        private void RefreshDecision(GameState state)
        {
            Decision? pending = state.GetDecision();

            if (pending == null || !controller.IsLocallyControlled(pending.GetPlayer()))
            {
                decision.Hide();

                return;
            }

            bool wasOpen = decision.IsOpen();

            decision.Show(pending);

            if (!wasOpen) decision.Open(windowLayer, GetBoardTopLeft() + new Vector2(60, 60));
        }

        private static string GetResultText(GameState state) => state.GetResult() switch
        {
            GameResult.Draw      => "Draw!",
            GameResult.Stalemate => "Stalemate!",
            _                    => $"{state.GetWinner()} wins!"
        };

        private void RefreshGameOver(GameState state)
        {
            if (!state.IsOver())
            {
                gameOver.Close();

                return;
            }

            gameOverLabel.text = GetResultText(state);

            newGameButton.SetEnabled(controller.CanStartNewGame());
            newGameButton.text = controller.CanStartNewGame() ? "New game" : "Waiting for host";

            gameOver.Open(windowLayer, GetBoardTopLeft() + new Vector2(80, 120));
        }

        private void ShowToast(Announcement announcement)
        {
            VisualElement toast = new() { pickingMode = PickingMode.Ignore };
            toast.AddToClassList("toast");

            Sprite? icon = sprites.Get(announcement.Icon);

            if (icon != null) toast.Add(UISprites.CreateImage(icon, "toast__icon"));

            Label text = new(announcement.Text) { pickingMode = PickingMode.Ignore };
            text.AddToClassList("toast__text");
            toast.Add(text);

            toastLayer.Add(toast);

            long duration = (long)(toastDuration * 1000);

            toast.schedule.Execute(() => toast.AddToClassList("toast--fading")).StartingIn(duration - 400);
            toast.schedule.Execute(toast.RemoveFromHierarchy).StartingIn(duration);
        }

        /// <summary>
        /// Keeps the side panels against the board's edges.
        /// </summary>
        private void PositionPanels()
        {
            if (root?.panel == null) return;

            Bounds bounds = boardView.GetWorldBounds();

            if (bounds.size == Vector3.zero) return;

            Vector2 topLeft     = WorldToPanel(new Vector3(bounds.min.x, bounds.max.y));
            Vector2 bottomRight = WorldToPanel(new Vector3(bounds.max.x, bounds.min.y));

            leftPanel.style.left = Mathf.Max(0, topLeft.x - panelGap - leftPanel.resolvedStyle.width);
            leftPanel.style.top  = topLeft.y;

            rightPanel.style.left = bottomRight.x + panelGap;
            rightPanel.style.top  = topLeft.y;
        }

        private Vector2 GetBoardTopLeft()
        {
            Bounds bounds = boardView.GetWorldBounds();

            return WorldToPanel(new Vector3(bounds.min.x, bounds.max.y));
        }

        private Vector2 WorldToPanel(Vector3 world)
        {
            Vector3 screen = cam.WorldToScreenPoint(world);

            return RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(screen.x, cam.pixelHeight - screen.y));
        }

        private bool IsPointerOverUI(Vector2 screen)
        {
            if (root?.panel == null) return false;

            Vector2       position = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(screen.x, cam.pixelHeight - screen.y));
            VisualElement? picked  = root.panel.Pick(position);

            return picked != null && picked != root && root.Contains(picked);
        }
    }
}
