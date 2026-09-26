using Boards;
using Gameplay.Decisions;

namespace Pieces.NPCs
{
    #nullable enable

    public class Angel : Npc
    {
        public const string ID = "angel";

        public override string GetId() => ID;

        public override int GetWidth () => 3;
        public override int GetHeight() => 3;

        /// <summary>
        /// Asks the killer's owner whether to free it as an <see cref="AggroAngel"/>.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            board.GetGame().PushDecision(new FreeAngelDecision(board.GetGame().GetResponsiblePlayer(killer), position));

            return false;
        }
    }
}
