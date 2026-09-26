using Boards;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Gameplay.Commands
{
    #nullable enable

    /// <summary>
    /// Also ends the turn.
    /// </summary>
    public class MoveCommand : GameCommand
    {
        public const string ID = "move";

        private readonly Position from;
        private readonly Position to;

        /// <param name="from">Any square the piece covers.</param>
        /// <param name="to">The piece's new bottom left, on the same board.</param>
        public MoveCommand(Piece.Color player, Position from, Position to) : base(player)
        {
            this.from = from;
            this.to   = to;
        }

        public override string GetId() => ID;

        public Position GetFrom() => from;
        public Position GetTo  () => to;

        public override bool IsValid(GameState state)
        {
            if (from.GetLocation() != to.GetLocation()) return false;

            if (!state.TryGetBoard(from.GetLocation(), out Board? board)) return false;

            Piece? piece = board.GetPieceAt(from);

            if (piece            == null  ) return false;
            if (piece.GetColor() != player) return false;

            return board.IsValidMove(piece, to);
        }

        public override void Execute(GameState state)
        {
            Board board = state.GetBoard(from.GetLocation());

            board.MovePiece(board.GetPieceAt(from)!, to);

            state.EndTurn();
        }

        protected override void SerializeData(JObject obj)
        {
            obj["from"] = from.Serialize();
            obj["to"]   = to.Serialize();
        }

        public static MoveCommand DeserializeData(JObject obj) => new(
            DeserializePlayer(obj),
            Position.Deserialize(obj["from"]!),
            Position.Deserialize(obj["to"]!)
        );
    }
}
