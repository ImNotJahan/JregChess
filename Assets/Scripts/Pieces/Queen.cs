using Boards;
using Util;

namespace Pieces
{
    public class Queen : Piece
    {
        public const string ID = "queen";

        public Queen(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to) =>
            Movement.IsClearLine(board, position, to);
    }
}
