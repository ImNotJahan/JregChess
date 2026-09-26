using Boards;

namespace Pieces.NPCs
{
    public class Coin : Npc
    {
        public const string ID = "coin";

        public override string GetId() => ID;

        public override int GetBounty(Board board) => 4;
    }
}
