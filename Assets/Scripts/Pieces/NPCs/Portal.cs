using System;
using Boards;
using Newtonsoft.Json.Linq;

namespace Pieces.NPCs
{
    #nullable enable

    public class Portal : Npc
    {
        public const string ID = "portal";

        private BoardType? target;

        /// <param name="target">
        /// Null to send pieces to Heaven, or to Hell from Heaven.
        /// </param>
        public Portal(BoardType? target = null)
        {
            this.target = target;
        }

        public override string GetId() => ID;

        /// <summary>
        /// Sends the killer to the same square on another board.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (killer == null) return false;

            BoardType destination = target ??
                (board.GetBoardType() == BoardType.Heaven ? BoardType.Hell : BoardType.Heaven);

            board.GetGame().Transfer(killer, destination, position.ChangeBoard(destination));

            return false;
        }

        protected override void SerializeState(JObject obj) => obj["target"] = target?.ToString();

        protected override void DeserializeState(JObject obj)
        {
            string? name = obj.Value<string?>("target");

            target = name == null ? null : Enum.Parse<BoardType>(name);
        }
    }
}
