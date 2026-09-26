using Boards;

namespace Pieces.NPCs
{
    #nullable enable

    public class Bomb : Npc
    {
        public const string ID = "bomb";

        public override string GetId() => ID;

        public override string GetDescription() => "Goes boom";

        public override bool Kill(Board board, Piece? killer)
        {
            board.Explode(position, this);

            return true;
        }
    }
}
