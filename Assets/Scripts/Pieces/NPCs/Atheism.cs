using Boards;
using Gameplay.Decisions;

namespace Pieces.NPCs
{
    #nullable enable

    public class Atheism : Npc
    {
        public const string ID = "atheism";

        public override string GetId() => ID;

        public override string GetDescription() => "";

        public override int GetWidth () => 2;
        public override int GetHeight() => 2;

        /// <summary>
        /// Survives, offering the killer's owner an <see cref="AtheismDecision"/>.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            board.GetGame().PushDecision(new AtheismDecision(board.GetGame().GetResponsiblePlayer(killer)));

            return false;
        }
    }
}
