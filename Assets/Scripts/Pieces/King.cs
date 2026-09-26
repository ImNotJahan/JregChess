using Boards;
using Util;

namespace Pieces
{
    public class King : Piece
    {
        public const string ID = "king";

        public King(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "More you got, harder you are to defeat!";

        public override bool IsRoyal() => true;

        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return Movement.IsKingMove(dx, dy);
        }
    }
}
