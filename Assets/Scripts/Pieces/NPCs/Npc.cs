using Boards;
using Util;

namespace Pieces.NPCs
{
    public abstract class Npc : Piece
    {
        protected Npc() : base(Color.NPC) {}

        public override bool IsValidMove(Board board, Position to) => false;

        public override BoardType? GetAfterlife() => null;

        public override int GetBounty(Board board) => 0;
    }
}
