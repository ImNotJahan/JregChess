using System;
using Boards;
using Util;

namespace Pieces.Shop
{
    #nullable enable

    public class Zebra : Knight
    {
        public new const string ID = "zebra";

        public Zebra(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "Like a horse, but more violent";

        /// <summary>
        /// Captures the two pieces along the long side of its move, including its
        /// own.
        /// </summary>
        public override void OnMoved(Board board, Position from)
        {
            int dx = position.GetX() - from.GetX();
            int dy = position.GetY() - from.GetY();

            (int stepX, int stepY) = Math.Abs(dx) > Math.Abs(dy)
                ? (Math.Sign(dx), 0)
                : (0, Math.Sign(dy));

            for (int i = 1; i <= 2; i++)
            {
                Piece? target = board.GetPieceAt(from.MoveBy(stepX * i, stepY * i));

                if (target != null && target != this) board.Capture(target, this);
            }
        }
    }
}
