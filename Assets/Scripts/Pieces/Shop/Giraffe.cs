using System;
using Boards;
using Util;

namespace Pieces.Shop
{
    public class Giraffe : Piece
    {
        public const string ID = "giraffe";

        public Giraffe(Color color = default) : base(color) {}

        public override string GetId() => ID;

        /// <summary>
        /// Like a knight, but three by one.
        /// </summary>
        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return (Math.Abs(dx) == 3 && Math.Abs(dy) == 1) ||
                   (Math.Abs(dx) == 1 && Math.Abs(dy) == 3);
        }
    }
}
