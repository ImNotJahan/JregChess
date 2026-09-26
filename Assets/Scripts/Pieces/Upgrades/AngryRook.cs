using Boards;
using Util;

namespace Pieces.Upgrades
{
    #nullable enable

    public class AngryRook : Piece
    {
        public const string ID = "angry-rook";

        public AngryRook(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "Nothing'll stop its charge";

        /// <summary>
        /// Rook moves which can jump over at most one piece.
        /// </summary>
        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return Movement.IsStraight(dx, dy) && Movement.CountBetween(board, position, to) <= 1;
        }

        /// <summary>
        /// Captures the pieces it jumped over, including its own.
        /// </summary>
        public override void OnMoved(Board board, Position from)
        {
            foreach (Position square in Movement.GetSquaresBetween(from, position))
            {
                Piece? target = board.GetPieceAt(square);

                if (target != null) board.Capture(target, this);
            }
        }
    }
}
