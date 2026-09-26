using Boards;
using Pieces;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    /// <summary>
    /// Kill any piece on the Normal board.
    /// </summary>
    public class SmiteDecision : Decision
    {
        public const string ID = "smite";

        public SmiteDecision(Piece.Color player) : base(player) {}

        public override string GetId() => ID;

        public override string GetTitle() => "Pick a piece to smite";

        public override string? GetIcon() => "devil";

        public override BoardType? GetTargetBoard() => BoardType.Normal;

        public override bool IsValid(GameState game, int option, Position? target) =>
            target is Position square &&
            square.GetLocation() == BoardType.Normal &&
            game.TryGetBoard(BoardType.Normal, out Board? normal) &&
            normal.HasPieceAt(square);

        public override void Resolve(GameState game, int option, Position? target)
        {
            Board board = game.GetBoard(BoardType.Normal);

            board.Capture(board.GetPieceAt(target!.Value)!);
        }
    }
}
