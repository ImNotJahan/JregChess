using Boards;

namespace Pieces.NPCs
{
    #nullable enable

    public class Whirlpool : Npc
    {
        public const string ID = "whirlpool";

        public override string GetId() => ID;

        /// <summary>
        /// Survives, taking the killer off the board.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (killer != null) board.RemovePiece(killer);

            return false;
        }
    }
}
