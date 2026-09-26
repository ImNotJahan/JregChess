using Boards;
using Util;

namespace Pieces
{
    public class Bishop : Piece
    {
        public const string ID = "bishop";

        public Bishop(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return Movement.IsDiagonal(dx, dy) && Movement.IsClearLine(board, position, to);
        }
    }
}
