using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apocryphon.Persistence;
using Boards;
using Gameplay.Commands;
using Gameplay.Decisions;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Gameplay
{
    #nullable enable

    public class GameState : ISerializable
    {
        public const int StartingGold = 5;

        public event Action<GameCommand>? CommandExecuted;
        public event Action<Piece.Color>? TurnChanged;
        public event Action<Announcement>? Announced;

        private readonly Dictionary<BoardType, Board>   boards    = new();
        private readonly List<GameCommand>              history   = new();
        private readonly Dictionary<Piece.Color, int>   gold      = new();
        private readonly List<Rule>                     rules     = new();
        private readonly List<Rule>                     rulePool  = new();
        /// <summary>
        /// The first is the one being made.
        /// </summary>
        private readonly List<Decision>                 decisions = new();

        private Rng          random = new(0);
        private Piece.Color  turn   = Piece.Color.White;
        private int          turnNumber;
        private int          turnsSinceNewRule;
        private Piece.Color  ruleChooser = Piece.Color.White;
        private Piece.Color? winner;
        private GameResult   result;

        private GameState() {}

        public static GameState CreateNew(ulong? seed = null)
        {
            GameState state = new()
            {
                random = new Rng(seed ?? (ulong)DateTime.UtcNow.Ticks)
            };

            foreach (BoardType type in Enum.GetValues(typeof(BoardType)))
                state.boards[type] = new Board(state, type);

            state.gold[Piece.Color.White] = StartingGold;
            state.gold[Piece.Color.Black] = StartingGold;

            state.rules   .AddRange(Rules.Starting);
            state.rulePool.AddRange(Rules.Selectable);

            Setup.SetupNormal(state.GetBoard(BoardType.Normal));
            Setup.SetupHeaven(state.GetBoard(BoardType.Heaven));
            Setup.SetupHell(state.GetBoard(BoardType.Hell));

            return state;
        }

        public static Piece.Color GetOpponent(Piece.Color player) =>
            player == Piece.Color.White ? Piece.Color.Black : Piece.Color.White;

        public IEnumerable<Board>         GetBoards    () => boards.Values;
        public Piece.Color                GetTurn      () => turn;
        public int                        GetTurnNumber() => turnNumber;
        public IReadOnlyList<GameCommand> GetHistory   () => history;
        public IReadOnlyList<Rule>        GetRules     () => rules;
        public IReadOnlyList<Rule>        GetRulePool  () => rulePool;
        public Rng                        GetRandom    () => random;
        public Piece.Color?               GetWinner    () => winner;
        public GameResult                 GetResult    () => result;
        public bool                       IsOver       () => result != GameResult.Ongoing;
        public Decision?                  GetDecision  () => decisions.Count > 0 ? decisions[0] : null;

        /// <summary>
        /// Throws if the board has been destroyed.
        /// </summary>
        public Board GetBoard(BoardType type) => boards[type];

        public bool HasBoard(BoardType type) => boards.ContainsKey(type);

        public bool TryGetBoard(BoardType type, [NotNullWhen(true)] out Board? board) =>
            boards.TryGetValue(type, out board);

        public Board? FindBoard(Piece piece)
        {
            foreach (Board board in boards.Values.Where(board => board.Contains(piece))) return board;

            return null;
        }

        public void DestroyBoard(BoardType type)
        {
            if (!boards.Remove(type)) return;

            Announce($"{type} has been destroyed!");

            CheckKings();
        }

        public bool HasKing(Piece.Color player) =>
            boards.Values.Any(board => board.GetPieces().Any(piece => piece.IsRoyal() && piece.GetColor() == player));

        private void CheckKings()
        {
            if (IsOver()) return;

            bool whiteLost = !HasKing(Piece.Color.White);
            bool blackLost = !HasKing(Piece.Color.Black);

            if (whiteLost && blackLost)
            {
                result = GameResult.Draw;

                Announce("Draw!");
            }
            else if (whiteLost || blackLost)
            {
                result = GameResult.Win;
                winner = whiteLost ? Piece.Color.Black : Piece.Color.White;

                Announce($"{winner} wins!");
            }
        }

        private void CheckStalemate()
        {
            if (IsOver()) return;

            if (Actions.HasAny(this, GetDecision()?.GetPlayer() ?? turn)) return;

            result = GameResult.Stalemate;

            Announce("Stalemate!");
        }

        public int GetGold(Piece.Color player) => gold.TryGetValue(player, out int amount) ? amount : 0;

        public void AddGold(Piece.Color player, int amount)
        {
            if (player == Piece.Color.NPC) return;

            gold[player] = GetGold(player) + amount;
        }

        public bool HasRule(Rule rule) => rules.Contains(rule);

        /// <summary>
        /// Applies the rule's effects, and keeps it active unless it's an event.
        /// </summary>
        public void AddRule(Rule rule)
        {
            rulePool.Remove(rule);

            if (!Rules.IsEvent(rule)) rules.Add(rule);

            Announce($"New rule: {Rules.GetDescription(rule)}");

            Rules.Apply(rule, this);
        }

        public void RemoveRule(Rule rule) => rules.Remove(rule);

        /// <summary>
        /// Removes the rule, returning if it was active.
        /// </summary>
        public bool ConsumeRule(Rule rule) => rules.Remove(rule);

        public void PushDecision(Decision decision) => decisions.Add(decision);

        /// <summary>
        /// Only for use by <see cref="ResolveDecisionCommand"/>.
        /// </summary>
        public void ResolveDecision(int option, Position? target)
        {
            Decision decision = decisions[0];

            decisions.RemoveAt(0);

            decision.Resolve(this, option, target);
        }

        public void Announce(string text, string? icon = null) =>
            Announced?.Invoke(new Announcement(text, icon));

        /// <summary>
        /// The killer's owner, or whoever's turn it is if the killer isn't a player's.
        /// </summary>
        public Piece.Color GetResponsiblePlayer(Piece? killer) => killer != null 
                                                               && killer.IsPlayerPiece() 
                                                                ? killer.GetColor() 
                                                                : turn;

        /// <summary>
        /// Moves the piece onto another board, capturing whatever is in the way. If it
        /// doesn't fit, it's put back.
        /// </summary>
        public bool Transfer(Piece piece, BoardType to, Position anchor)
        {
            if (!boards.TryGetValue(to, out Board? destination)) return false;

            Board?   source   = FindBoard(piece);
            Position original = piece.GetPosition();

            source?.RemovePiece(piece);

            if (destination.ForcePlace(piece, anchor.ChangeBoard(to))) return true;

            source?.AddPiece(piece, original);

            return false;
        }

        public bool CanExecute(GameCommand command)
        {
            if (IsOver()) return false;

            if (GetDecision() != null)
                return command is ResolveDecisionCommand && command.IsValid(this);

            return command.GetPlayer() == turn && command.IsValid(this);
        }

        /// <summary>
        /// Runs and records the command if it's valid.
        /// </summary>
        /// <returns>If the command was run</returns>
        public bool Execute(GameCommand command)
        {
            if (!CanExecute(command)) return false;

            command.Execute(this);
            history.Add(command);

            CheckKings();
            CheckStalemate();

            CommandExecuted?.Invoke(command);

            return true;
        }

        /// <summary>
        /// Runs the command on a copy, leaving this state untouched. The copy has no
        /// event listeners.
        /// </summary>
        /// <returns>The copy, or null if the command isn't valid</returns>
        public GameState? ExecuteOnCopy(GameCommand command)
        {
            if (!CanExecute(command)) return null;

            GameState copy = Clone();

            copy.Execute(command);

            return copy;
        }

        /// <summary>
        /// A deep copy, without event listeners. Commands and decisions are immutable,
        /// so they're shared.
        /// </summary>
        public GameState Clone()
        {
            GameState copy = new()
            {
                random            = new Rng(random.GetState()),
                turn              = turn,
                turnNumber        = turnNumber,
                turnsSinceNewRule = turnsSinceNewRule,
                ruleChooser       = ruleChooser,
                winner            = winner,
                result            = result
            };

            foreach ((BoardType type, Board board) in boards)
                copy.boards[type] = board.Clone(copy);

            foreach ((Piece.Color player, int amount) in gold)
                copy.gold[player] = amount;

            copy.history  .AddRange(history);
            copy.rules    .AddRange(rules);
            copy.rulePool .AddRange(rulePool);
            copy.decisions.AddRange(decisions);

            return copy;
        }

        /// <summary>
        /// Only for use by commands. Runs every piece's end of turn behaviour, and
        /// offers a new rule once enough turns have passed.
        /// </summary>
        public void EndTurn()
        {
            turn = GetOpponent(turn);
            turnNumber++;
            turnsSinceNewRule++;

            foreach (Board board in new List<Board>(boards.Values))
            foreach (Piece piece in new List<Piece>(board.GetPieces()).Where(piece => board.Contains(piece))) piece.OnTurnEnded(board);

            if (rulePool.Count > 0 && turnsSinceNewRule >= rules.Count * 2)
            {
                turnsSinceNewRule = 0;

                PushDecision(new RuleDecision(ruleChooser));

                ruleChooser = GetOpponent(ruleChooser);
            }

            TurnChanged?.Invoke(turn);
        }

        /// <summary>
        /// Only for use by <see cref="Board"/>, after the piece has been removed.
        /// </summary>
        public void OnPieceKilled(Board board, Piece piece, Piece? killer)
        {
            if (killer != null && killer.IsPlayerPiece())
                AddGold(killer.GetColor(), piece.GetBounty(board));

            if (board.GetBoardType() != BoardType.Normal) return;
            if (piece.GetAfterlife() is not BoardType afterlife) return;
            if (!boards.TryGetValue(afterlife, out Board? destination)) return;

            piece.Heal();

            destination.ForcePlace(piece, piece.GetPosition().ChangeBoard(afterlife));
        }

        public JToken Serialize()
        {
            JArray boardArray = new();
            foreach (Board board in boards.Values)
                boardArray.Add(board.Serialize());

            JArray historyArray = new();
            foreach (GameCommand command in history)
                historyArray.Add(command.Serialize());

            JArray decisionArray = new();
            foreach (Decision decision in decisions)
                decisionArray.Add(decision.Serialize());

            JObject goldObject = new();
            foreach ((Piece.Color player, int amount) in gold)
                goldObject[player.ToString()] = amount;

            return new JObject
            {
                ["turn"]              = turn.ToString(),
                ["turnNumber"]        = turnNumber,
                ["turnsSinceNewRule"] = turnsSinceNewRule,
                ["ruleChooser"]       = ruleChooser.ToString(),
                ["winner"]            = winner?.ToString(),
                ["result"]            = result.ToString(),
                ["random"]            = random.GetState().ToString(),
                ["gold"]              = goldObject,
                ["rules"]             = SerializeRules(rules),
                ["rulePool"]          = SerializeRules(rulePool),
                ["decisions"]         = decisionArray,
                ["boards"]            = boardArray,
                ["history"]           = historyArray
            };
        }

        private static JArray SerializeRules(List<Rule> list)
        {
            JArray array = new();

            foreach (Rule rule in list)
                array.Add(rule.ToString());

            return array;
        }

        public static GameState Deserialize(JToken token)
        {
            string? winnerName = token.Value<string?>("winner");
            string? resultName = token.Value<string?>("result");

            GameState state = new()
            {
                turn              = Enum.Parse<Piece.Color>(token.Value<string>("turn")!),
                turnNumber        = token.Value<int>("turnNumber"),
                turnsSinceNewRule = token.Value<int>("turnsSinceNewRule"),
                ruleChooser       = Enum.Parse<Piece.Color>(token.Value<string>("ruleChooser")!),
                winner            = winnerName == null ? null : Enum.Parse<Piece.Color>(winnerName),
                result            = resultName != null ? Enum.Parse<GameResult>(resultName)
                                  : winnerName != null ? GameResult.Win
                                  : GameResult.Ongoing,
                random            = new Rng(ulong.Parse(token.Value<string>("random")!))
            };

            foreach (JProperty property in ((JObject)token["gold"]!).Properties())
                state.gold[Enum.Parse<Piece.Color>(property.Name)] = property.Value.Value<int>();

            foreach (JToken rule in token["rules"]!)
                state.rules.Add(Enum.Parse<Rule>(rule.Value<string>()!));

            foreach (JToken rule in token["rulePool"]!)
                state.rulePool.Add(Enum.Parse<Rule>(rule.Value<string>()!));

            foreach (JToken decisionToken in token["decisions"]!)
                state.decisions.Add(Decision.Deserialize(decisionToken));

            foreach (JToken boardToken in token["boards"]!)
            {
                Board board = Board.Deserialize(state, boardToken);

                state.boards[board.GetBoardType()] = board;
            }

            // only a record, not replayed
            foreach (JToken commandToken in token["history"]!)
                state.history.Add(GameCommand.Deserialize(commandToken));

            return state;
        }
    }
}
