using Boards;
using Util;

namespace Pieces
{
    public class Rook : Piece
    {
        public const string ID = "rook";

        public Rook(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "Imagine getting hit by a wall";

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return Movement.IsStraight(dx, dy) && Movement.IsClearLine(board, position, to);
        }
    }
}
