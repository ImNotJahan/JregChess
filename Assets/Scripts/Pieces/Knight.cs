using Boards;
using Util;

namespace Pieces
{
    public class Knight : Piece
    {
        public const string ID = "knight";

        public Knight(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return Movement.IsKnightMove(dx, dy);
        }
    }
}
