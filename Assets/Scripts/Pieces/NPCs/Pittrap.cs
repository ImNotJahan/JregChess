using Boards;

namespace Pieces.NPCs
{
    #nullable enable

    public class Pittrap : Npc
    {
        public const string ID = "pittrap";

        public override string GetId() => ID;

        public override string GetDescription() => "";

        /// <summary>
        /// Takes the killer off the board along with itself.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (killer != null) board.RemovePiece(killer);

            return true;
        }
    }
}
