using System.Collections.Generic;
using Boards;
using Util;

namespace Pieces.Upgrades
{
    #nullable enable

    public class RookTower : Piece
    {
        public const string ID = "rook-tower";

        public RookTower(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to) => false;

        /// <summary>
        /// Switches the side of the pieces around it when it dies.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (!base.Kill(board, killer)) return false;

            List<Piece> around = new();

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    Piece? piece = board.GetPieceAt(position.MoveBy(dx, dy));

                    if (piece != null && piece != this && piece.IsPlayerPiece() && !around.Contains(piece))
                        around.Add(piece);
                }
            }

            foreach (Piece piece in around)
                piece.SetColor(piece.GetColor() == Color.White ? Color.Black : Color.White);

            return true;
        }
    }
}
