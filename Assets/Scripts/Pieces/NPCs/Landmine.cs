using Boards;

namespace Pieces.NPCs
{
    #nullable enable

    public class Landmine : Npc
    {
        public const string ID = "landmine";

        public override string GetId() => ID;

        public override string GetDescription() => "Goes boom when stepped on";

        /// <summary>
        /// Explodes, taking the killer off the board.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            board.Explode(position, this);

            if (killer != null) board.RemovePiece(killer);

            return true;
        }
    }
}
