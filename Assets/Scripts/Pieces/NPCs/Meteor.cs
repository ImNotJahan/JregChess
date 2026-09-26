using Boards;
using Util;

namespace Pieces.NPCs
{
    public class Meteor : Npc
    {
        public const string ID = "meteor";

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to) => true;

        /// <summary>
        /// Moves diagonally down and to the right.
        /// </summary>
        public override void OnTurnEnded(Board board) => board.MovePiece(this, position.MoveBy(1, -1));

        /// <summary>
        /// Explodes when it hits something, destroying itself.
        /// </summary>
        public override void OnCapture(Board board, Piece captured, Position from)
        {
            board.Explode(position, this);
            board.RemovePiece(this);
        }
    }
}
