using Boards;
using Util;

namespace Pieces.Upgrades
{
    public class Centaur : Pawn
    {
        public new const string ID = "centaur";

        public Centaur(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "Half-pawn, half-knight";

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return base.IsValidMove(board, to) || Movement.IsKnightMove(dx, dy);
        }
    }
}
