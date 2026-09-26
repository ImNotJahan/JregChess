using Boards;

namespace Pieces.Upgrades
{
    public class SuicideBomber : Pawn
    {
        #nullable enable

        public new const string ID = "suicide-bomber";

        public SuicideBomber(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override BoardType? GetAfterlife() => BoardType.Heaven;

        /// <summary>
        /// Always dies, exploding and taking the killer off the board.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            board.Explode(position, this);

            if (killer != null) board.RemovePiece(killer);

            return true;
        }
    }
}
