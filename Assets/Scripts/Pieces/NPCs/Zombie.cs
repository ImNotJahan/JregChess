using Boards;
using Newtonsoft.Json.Linq;
using Util;

namespace Pieces.NPCs
{
    public class Zombie : Npc
    {
        public const string ID = "zombie";

        private bool movingRight;

        public Zombie(bool movingRight = true)
        {
            this.movingRight = movingRight;
        }

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to) => true;

        /// <summary>
        /// Walks one square sideways, turning around at the edges and at other
        /// zombies.
        /// </summary>
        public override void OnTurnEnded(Board board)
        {
            Position to = position.MoveBy(movingRight ? 1 : -1, 0);

            if (board.GetPieceAt(to) is Zombie)
            {
                movingRight = !movingRight;

                to = position.MoveBy(movingRight ? 1 : -1, 0);
            }

            board.MovePiece(this, to);

            if      (to.GetX() >= board.GetWidth() - 1) movingRight = false;
            else if (to.GetX() <= 0)                    movingRight = true;
        }

        /// <summary>
        /// Leaves a new zombie where it was.
        /// </summary>
        public override void OnCapture(Board board, Piece captured, Position from) => board.AddPiece(new Zombie(movingRight), from);

        protected override void SerializeState(JObject obj) => obj["movingRight"] = movingRight;

        protected override void DeserializeState(JObject obj) =>
            movingRight = obj.Value<bool>("movingRight");
    }
}
