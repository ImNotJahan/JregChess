using Boards;
using Util;

namespace Pieces.Upgrades
{
    public class KnightQueen : Queen
    {
        public new const string ID = "knight-queen";

        public KnightQueen(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "What if the queen could jump?";

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return base.IsValidMove(board, to) || Movement.IsKnightMove(dx, dy);
        }
    }
}
