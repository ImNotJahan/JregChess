using System;
using Boards;
using Util;

namespace Pieces.Upgrades
{
    public class SuperKing : Piece
    {
        public const string ID = "super-king";

        public SuperKing(Color color = default) : base(color) {}

        protected override void Initialize() => maxHealth = 2;

        public override string GetId() => ID;

        public override int GetWidth () => 2;
        public override int GetHeight() => 2;

        public override bool IsRoyal() => true;

        /// <summary>
        /// Up to two squares in any direction, jumping over pieces.
        /// </summary>
        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return Math.Abs(dx) <= 2 && Math.Abs(dy) <= 2;
        }
    }
}
