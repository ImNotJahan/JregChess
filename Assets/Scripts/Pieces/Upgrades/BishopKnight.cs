using Boards;
using Util;

namespace Pieces.Upgrades
{
    public class BishopKnight : Bishop
    {
        public new const string ID = "bishop-knight";

        public BishopKnight(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return base.IsValidMove(board, to) || Movement.IsKnightMove(dx, dy);
        }
    }
}
