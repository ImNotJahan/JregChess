using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AI;
using Boards;
using Gameplay;
using Gameplay.Commands;
using Gameplay.Decisions;
using Networking;
using Pieces;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Views
{
    #nullable enable

    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// What clicking a square does.
        /// </summary>
        public enum Mode
        {
            Move,
            /// <summary>Places the item being bought.</summary>
            Buy,
            /// <summary>Upgrades the clicked piece.</summary>
            Upgrade,
            /// <summary>Picks the square for the current decision.</summary>
            Target
        }

        /// <summary>
        /// Anything shown in the UI may have changed.
        /// </summary>
        public event Action?               Changed;
        public event Action<Announcement>? Announced;
        /// <summary>
        /// The online opponent is gone, with the reason.
        /// </summary>
        public event Action<string>?       OpponentLost;

        [SerializeField] private BoardView boardView   = null!;
        [SerializeField] private Camera    inputCamera = null!;
        [Tooltip("Seconds between the bot's actions")]
        [SerializeField] private float     botDelay    = 0.5f;

        private GameState state       = null!;
        private BoardType viewedBoard = BoardType.Normal;
        private Mode      mode        = Mode.Move;
        /// <summary>
        /// Id of the item being bought or upgrade being applied.
        /// </summary>
        private string?   modeItem;

        private Piece?   selected;
        /// <summary>
        /// The clicked square of the selected piece, relative to its bottom left.
        /// </summary>
        private Position grabOffset;

        private Func<Vector2, bool>? isPointerBlocked;

        private Bot?        bot;
        private Piece.Color botColor;
        private float       botReadyTime;
        /// <summary>
        /// The bot gave nothing valid to run. Cleared when a command runs.
        /// </summary>
        private bool        botStuck;

        private Task<GameCommand?>?      botTask;
        private CancellationTokenSource? botCancellation;

        private OnlineSession? online;

        private void Awake()
        {
            if (inputCamera == null) inputCamera = Camera.main!;

            bot      = MatchSetup.GetBot();
            botColor = MatchSetup.GetBotColor();
            online   = MatchSetup.GetOnline();

            LoadState(GameState.CreateNew());

            if (online == null) return;

            online.Disconnected += OnDisconnected;

            ReceiveOnline();
        }

        private void OnDestroy()
        {
            CancelBot();
            Unsubscribe();

            if (online == null) return;

            online.Disconnected -= OnDisconnected;
            online.Leave();
        }

        private void Update()
        {
            RunBot();
            ReceiveOnline();

            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true ||
                Mouse.current?.rightButton.wasPressedThisFrame == true)
                CancelMode();

            Pointer? pointer = Pointer.current;

            if (pointer == null || !pointer.press.wasReleasedThisFrame) return;

            Vector2 screen = pointer.position.ReadValue();

            if (isPointerBlocked != null && isPointerBlocked(screen)) return;

            Vector3 world = inputCamera.ScreenToWorldPoint(screen);

            if (boardView.TryGetSquare(world, out Position square)) HandleClick(square);
            else if (mode != Mode.Target)                           CancelMode();
        }

        public GameState GetState      () => state;
        public BoardType GetViewedBoard() => viewedBoard;
        public Mode      GetMode       () => mode;
        public string?   GetModeItem   () => modeItem;
        public Bot?      GetBot        () => bot;
        public bool      IsOnline      () => online != null;

        /// <summary>
        /// Online, only the host can.
        /// </summary>
        public bool CanStartNewGame() => online == null || online.IsHost();

        /// <summary>
        /// Whoever has to act next: the player making the pending decision, or else
        /// the player whose turn it is.
        /// </summary>
        public Piece.Color GetActingPlayer() => state.GetDecision()?.GetPlayer() ?? state.GetTurn();

        /// <summary>
        /// Whether this client's input acts for <paramref name="color"/>.
        /// </summary>
        public bool IsLocallyControlled(Piece.Color color)
        {
            if (color == Piece.Color.NPC) return false;
            if (online != null)           return color == online.GetLocalColor();

            return bot == null || color != botColor;
        }

        /// <summary>
        /// Clicks for which this returns true are ignored.
        /// </summary>
        public void SetPointerFilter(Func<Vector2, bool> isBlocked) => isPointerBlocked = isBlocked;

        public void LoadState(GameState newState)
        {
            CancelBot();
            Unsubscribe();

            state = newState;
            state.CommandExecuted += OnCommandExecuted;
            state.Announced       += OnAnnounced;

            mode     = Mode.Move;
            modeItem = null;

            botReadyTime = Time.time + botDelay;
            botStuck     = false;

            SyncWithState();
        }

        /// <summary>
        /// Online, the new game arrives as a message.
        /// </summary>
        public void NewGame()
        {
            if (!CanStartNewGame()) return;

            if (online != null) online.StartNewGame();
            else                LoadState(GameState.CreateNew());
        }

        /// <summary>
        /// Only changes this client's view, not the game.
        /// </summary>
        public void ShowBoard(BoardType type)
        {
            if (!state.HasBoard(type)) return;

            viewedBoard = type;
            selected    = null;

            boardView.Bind(state.GetBoard(type));

            RefreshHighlights();

            Changed?.Invoke();
        }

        public void BeginBuy(string item) => SetMode(Mode.Buy, item, BoardType.Normal);

        public void BeginUpgrade(string upgrade) => SetMode(Mode.Upgrade, upgrade, null);

        /// <summary>
        /// Back to moving pieces. Does nothing while a square must be picked for a
        /// decision.
        /// </summary>
        public void CancelMode()
        {
            if (mode == Mode.Target) return;

            SetMode(Mode.Move, null, null);
        }

        public bool ChooseOption(int option)
        {
            Decision? decision = state.GetDecision();

            return decision != null &&
                   IsLocallyControlled(decision.GetPlayer()) &&
                   Submit(new ResolveDecisionCommand(decision.GetPlayer(), option));
        }

        /// <summary>
        /// For commands made on this client.
        /// </summary>
        private bool Submit(GameCommand command)
        {
            if (online != null && command.GetPlayer() != online.GetLocalColor()) return false;

            if (!state.Execute(command)) return false;

            if (online != null) online.SendCommand(command);

            return true;
        }

        private void ReceiveOnline()
        {
            if (online == null) return;

            while (online.TryReceive(out OnlineSession.Message message))
            {
                if (message.Seed is ulong seed)
                {
                    LoadState(GameState.CreateNew(seed));
                }
                else if (message.Command is GameCommand command &&
                         (command.GetPlayer() == online.GetLocalColor() || !state.Execute(command)))
                {
                    Debug.LogWarning($"Opponent's {command.GetId()} command was invalid here, so the games are out of sync");
                }
            }
        }

        private void OnDisconnected(string reason) => OpponentLost?.Invoke(reason);

        private void RunBot()
        {
            if (bot == null) return;

            if (botTask != null)
            {
                if (botTask.IsCompleted) FinishBotTurn();

                return;
            }

            if (botStuck || state.IsOver()) return;
            if (GetActingPlayer() != botColor || Time.time < botReadyTime) return;

            Bot         thinker  = bot;
            Piece.Color color    = botColor;
            GameState   snapshot = state.Clone();

            botCancellation = new CancellationTokenSource();
            CancellationToken token = botCancellation.Token;

            botTask = Task.Run(() => thinker.HandleTurn(snapshot, color, token), token);
        }

        private void FinishBotTurn()
        {
            Task<GameCommand?> task = botTask!;

            botTask = null;
            botCancellation?.Dispose();
            botCancellation = null;

            if (task.IsFaulted) Debug.LogException(task.Exception!.GetBaseException());

            GameCommand? command = task.IsCompletedSuccessfully ? task.Result : null;

            if (command != null && Submit(command)) return;

            botStuck = true;

            Debug.LogWarning(command == null
                ? $"{bot!.GetName()} bot has nothing to do"
                : $"{bot!.GetName()} bot gave an invalid {command.GetId()} command");
        }

        private void CancelBot()
        {
            botCancellation?.Cancel();
            botCancellation = null;
            botTask         = null;
        }

        private void SetMode(Mode newMode, string? item, BoardType? board)
        {
            if (mode == Mode.Target && newMode != Mode.Target && state.GetDecision()?.GetTargetBoard() != null) return;
            if (newMode != Mode.Target && state.GetDecision() != null) return;
            if (newMode is Mode.Buy or Mode.Upgrade && !IsLocallyControlled(state.GetTurn())) return;

            mode     = newMode;
            modeItem = item;
            selected = null;

            if (board is BoardType type && type != viewedBoard) ShowBoard(type);

            RefreshHighlights();

            Changed?.Invoke();
        }

        private void HandleClick(Position square)
        {
            switch (mode)
            {
                case Mode.Target:
                    Decision? decision = state.GetDecision();

                    if (decision != null) Submit(new ResolveDecisionCommand(decision.GetPlayer(), 0, square));
                    break;

                case Mode.Buy:
                    string buying = modeItem!;

                    SetMode(Mode.Move, null, null);
                    Submit(new BuyCommand(state.GetTurn(), buying, square));
                    break;

                case Mode.Upgrade:
                    string upgrading = modeItem!;

                    SetMode(Mode.Move, null, null);
                    Submit(new UpgradeCommand(state.GetTurn(), upgrading, square));
                    break;

                default:
                    HandleMoveClick(square);
                    break;
            }
        }

        private void HandleMoveClick(Position square)
        {
            Board  board   = state.GetBoard(viewedBoard);
            Piece? clicked = board.GetPieceAt(square);

            if (selected != null)
            {
                Piece previous = selected;

                MoveCommand move = new(state.GetTurn(), selected.GetPosition(), square - grabOffset);

                Deselect();

                if (Submit(move)) return;

                if (clicked == previous) return;
            }

            if (clicked != null && CanSelect(clicked)) Select(clicked, square);
        }

        private bool CanSelect(Piece piece) =>
            state.GetDecision() == null &&
            !state.IsOver() &&
            piece.GetColor() == state.GetTurn() &&
            IsLocallyControlled(piece.GetColor());

        private void Select(Piece piece, Position square)
        {
            selected   = piece;
            grabOffset = square - piece.GetPosition();

            RefreshHighlights();
        }

        private void Deselect()
        {
            selected = null;

            RefreshHighlights();
        }

        private void RefreshHighlights()
        {
            boardView.ClearHighlights();

            if (!state.TryGetBoard(viewedBoard, out Board? board)) return;

            switch (mode)
            {
                case Mode.Move:
                    HighlightMoves(board);
                    break;

                case Mode.Buy:
                    HighlightBuySquares(board);
                    break;

                case Mode.Upgrade:
                    HighlightUpgradeable(board);
                    break;

                case Mode.Target:
                    if (state.GetDecision()?.GetTargetBoard() == viewedBoard)
                        foreach (Piece piece in board.GetPieces())
                            HighlightPiece(piece, BoardView.Highlight.Capture);
                    break;
            }
        }

        private void HighlightMoves(Board board)
        {
            if (selected == null || !board.Contains(selected)) return;

            HighlightPiece(selected, BoardView.Highlight.Selected);

            foreach (Position anchor in board.GetValidMoves(selected))
            {
                bool capture = board.GetPiecesUnder(selected, anchor).Count > 0;

                boardView.SetHighlight(
                    anchor + grabOffset,
                    capture ? BoardView.Highlight.Capture : BoardView.Highlight.Move
                );
            }
        }

        private void HighlightBuySquares(Board board)
        {
            if (!ShopCatalog.TryGet(modeItem!, out ShopItem item)) return;
            if (state.GetGold(state.GetTurn()) < item.Cost) return;

            Piece piece = PieceRegistry.Create(item.Id, item.Neutral ? Piece.Color.NPC : state.GetTurn());

            foreach (Position anchor in board.GetFreeAnchors(piece))
                if (new BuyCommand(state.GetTurn(), item.Id, anchor).IsValid(state))
                    boardView.SetHighlight(anchor, BoardView.Highlight.Move);
        }

        private void HighlightUpgradeable(Board board)
        {
            foreach (Piece piece in board.GetPieces())
            {
                UpgradeCommand upgrade = new(state.GetTurn(), modeItem!, piece.GetPosition());

                if (upgrade.IsValid(state)) HighlightPiece(piece, BoardView.Highlight.Move);
            }
        }

        private void HighlightPiece(Piece piece, BoardView.Highlight highlight)
        {
            foreach (Position covered in piece.GetFootprint(piece.GetPosition()))
                boardView.SetHighlight(covered, highlight);
        }

        /// <summary>
        /// Makes the view and mode match the state, such as when a board was destroyed
        /// or a decision needs a square picked.
        /// </summary>
        private void SyncWithState()
        {
            selected = null;

            BoardType? target = state.GetDecision()?.GetTargetBoard();

            if (target is BoardType targetBoard && state.HasBoard(targetBoard) &&
                IsLocallyControlled(state.GetDecision()!.GetPlayer()))
            {
                mode        = Mode.Target;
                modeItem    = null;
                viewedBoard = targetBoard;
            }
            else if (mode == Mode.Target || state.GetDecision() != null)
            {
                mode     = Mode.Move;
                modeItem = null;
            }

            List<BoardType> fallbacks = new() { viewedBoard, BoardType.Normal, BoardType.Heaven, BoardType.Hell };

            foreach (BoardType type in fallbacks)
            {
                if (!state.HasBoard(type)) continue;

                viewedBoard = type;

                break;
            }

            ShowBoard(viewedBoard);
        }

        private void OnCommandExecuted(GameCommand command)
        {
            botReadyTime = Time.time + botDelay;
            botStuck     = false;

            SyncWithState();
        }

        private void OnAnnounced(Announcement announcement) => Announced?.Invoke(announcement);

        private void Unsubscribe()
        {
            if (state == null) return;

            state.CommandExecuted -= OnCommandExecuted;
            state.Announced       -= OnAnnounced;
        }
    }
}
