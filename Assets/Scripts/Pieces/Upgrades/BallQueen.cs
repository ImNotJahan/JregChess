using System;
using Boards;
using Util;

namespace Pieces.Upgrades
{
    public class BallQueen : Queen
    {
        public new const string ID = "ball-queen";

        public BallQueen(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "What if the queen could *really* jump?";

        /// <summary>
        /// Queen moves, or jumps to the ring of squares three away.
        /// </summary>
        public override bool IsValidMove(Board board, Position to)
        {
            if (base.IsValidMove(board, to)) return true;

            (int dx, int dy) = GetOffset(to);

            int x = Math.Abs(dx);
            int y = Math.Abs(dy);

            return (y == 3 && x <= 1) || (x == 3 && y <= 1) || (x == 2 && y == 2);
        }
    }
}
