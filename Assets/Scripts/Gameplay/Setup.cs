using Boards;
using Pieces;
using Pieces.NPCs;
using Util;

namespace Gameplay
{
    public static class Setup
    {
        private static readonly string[] BackRank =
        {
            Rook.ID, Knight.ID, Bishop.ID, Queen.ID, King.ID, Bishop.ID, Knight.ID, Rook.ID
        };

        public static void SetupNormal(Board normal)
        {
            int top = normal.GetHeight() - 1;

            for (int x = 0; x < normal.GetWidth(); x++)
            {
                normal.AddPiece(new Pawn(Piece.Color.White), new Position(x, 1));
                normal.AddPiece(new Pawn(Piece.Color.Black), new Position(x, top - 1));

                string id = BackRank[x % BackRank.Length];

                normal.AddPiece(PieceRegistry.Create(id, Piece.Color.White), new Position(x, 0));
                normal.AddPiece(PieceRegistry.Create(id, Piece.Color.Black), new Position(x, top));
            }

            normal.AddPiece(new Coin(), new Position(0, 4));
            normal.AddPiece(new Coin(), new Position(7, 3));
        }

        public static void SetupHeaven(Board heaven)
        {
            heaven.AddPiece(new Coin(),    new Position(7, 7));
            heaven.AddPiece(new Portal(),  new Position(0, 0));
            heaven.AddPiece(new Angel(),   new Position(1, 4));
            heaven.AddPiece(new Atheism(), new Position(5, 4));
            heaven.AddPiece(new Church(),  new Position(6, 0));
        }

        public static void SetupHell(Board hell)
        {
            hell.AddPiece(new Coin(),   new Position(0, 0));
            hell.AddPiece(new Coin(),   new Position(5, 0));
            hell.AddPiece(new Coin(),   new Position(1, 5));
            hell.AddPiece(new Coin(),   new Position(3, 7));
            hell.AddPiece(new Coin(),   new Position(7, 7));
            hell.AddPiece(new Portal(), new Position(2, 2));
            hell.AddPiece(new Devil(),  new Position(4, 4));
        }
    }
}
