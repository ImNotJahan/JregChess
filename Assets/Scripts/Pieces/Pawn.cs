using System;
using Boards;
using Gameplay;
using Newtonsoft.Json.Linq;
using Util;

namespace Pieces
{
    public class Pawn : Piece
    {
        #nullable enable

        public const string ID = "pawn";

        /// <summary>
        /// Promotion is skipped while this id isn't registered.
        /// </summary>
        public const string PromotesTo = "queen";

        protected int firstMoveDistance = 2;

        protected bool moved;

        public Pawn(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public bool HasMoved() => moved;

        /// <summary>
        /// 1 or -1 on the y axis
        /// </summary>
        public int GetDirection() => color == Color.Black ? -1 : 1;

        public override bool IsValidMove(Board board, Position to)
        {
            int dx      = to.GetX() - position.GetX();
            int forward = (to.GetY() - position.GetY()) * GetDirection();

            // the board has already checked it's an enemy
            if (board.HasPieceAt(to)) return forward == 1 && Math.Abs(dx) == 1;

            if (dx != 0) return false;
            if (forward <= 0) return false;
            if (forward > (moved ? 1 : GetFirstMoveDistance(board))) return false;

            for (int i = 1; i < forward; i++)
                if (board.HasPieceAt(position.MoveBy(0, i * GetDirection())))
                    return false;

            return true;
        }

        private int GetFirstMoveDistance(Board board) =>
            board.GetGame().HasRule(Rule.PawnsMoveFour) ? 4 : firstMoveDistance;

        public override void OnMoved(Board board, Position from)
        {
            moved = true;

            if (!IsOnLastRank(board)) return;
            if (!PieceRegistry.IsRegistered(PromotesTo)) return;

            board.ReplacePiece(this, PieceRegistry.Create(PromotesTo, color));
        }

        private bool IsOnLastRank(Board board) =>
            color == Color.Black
                ? position.GetY() == 0
                : position.GetY() == board.GetHeight() - 1;

        protected override void SerializeState(JObject obj) => obj["moved"] = moved;

        protected override void DeserializeState(JObject obj) =>
            moved = obj.Value<bool>("moved");
    }
}
