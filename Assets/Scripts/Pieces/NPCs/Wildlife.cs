using Boards;
using Newtonsoft.Json.Linq;
using Util;

namespace Pieces.NPCs
{
    public class Wildlife : Npc
    {
        public const string ID = "wildlife";

        private bool movingRight;

        public Wildlife(bool movingRight = true)
        {
            this.movingRight = movingRight;
        }

        public override string GetId() => ID;

        public override string GetDescription() => "";

        /// <summary>
        /// Can't capture.
        /// </summary>
        public override bool IsValidMove(Board board, Position to) => !board.HasPieceAt(to);

        /// <summary>
        /// Walks one square sideways, turning around at the edges.
        /// </summary>
        public override void OnTurnEnded(Board board) =>
            board.MovePiece(this, position.MoveBy(movingRight ? 1 : -1, 0));

        public override void OnMoved(Board board, Position from)
        {
            if      (position.GetX() == board.GetWidth() - 1) movingRight = false;
            else if (position.GetX() == 0)                    movingRight = true;
        }

        protected override void SerializeState(JObject obj) => obj["movingRight"] = movingRight;

        protected override void DeserializeState(JObject obj) =>
            movingRight = obj.Value<bool>("movingRight");
    }
}
