using Boards;
using Gameplay.Decisions;

namespace Pieces.NPCs
{
    #nullable enable

    public class Devil : Npc
    {
        public const string ID = "devil";

        public override string GetId() => ID;

        public override string GetDescription() => "";

        public override int GetWidth () => 2;
        public override int GetHeight() => 2;

        /// <summary>
        /// Dies, offering the killer's owner a <see cref="DevilDecision"/>.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            board.GetGame().PushDecision(new DevilDecision(board.GetGame().GetResponsiblePlayer(killer)));

            return true;
        }
    }
}
