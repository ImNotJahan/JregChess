using System;
using Boards;
using Util;

namespace Pieces.Upgrades
{
    public class Unicorn : Knight
    {
        public new const string ID = "unicorn";

        public Unicorn(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "A knight who knows how to skewer";

        /// <summary>
        /// Knight moves, or jumps two squares straight.
        /// </summary>
        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return base.IsValidMove(board, to) ||
                   (dx == 0 && Math.Abs(dy) == 2) ||
                   (dy == 0 && Math.Abs(dx) == 2);
        }
    }
}
