using Boards;

namespace Pieces.NPCs
{
    #nullable enable

    public class AggroDevil : Npc
    {
        public const string ID = "aggro-devil";

        protected override void Initialize() => maxHealth = 6;

        public override string GetId() => ID;

        public override int GetWidth () => 2;
        public override int GetHeight() => 2;

        /// <summary>
        /// Captures the killer before taking damage.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (killer != null && board.Contains(killer)) board.Capture(killer, this);

            return base.Kill(board, killer);
        }
    }
}
