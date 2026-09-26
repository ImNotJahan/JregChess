using Boards;

namespace Pieces.NPCs
{
    public class Treasure : Npc
    {
        public const string ID = "treasure";

        public override string GetId() => ID;

        public override string GetDescription() => "";

        public override int GetBounty(Board board) => 15;
    }
}
