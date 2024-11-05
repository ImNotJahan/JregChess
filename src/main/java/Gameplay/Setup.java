package main.java.Gameplay;

import main.java.Board.Board;
import main.java.Pieces.*;
import main.java.Pieces.NPCs.*;
import main.java.Util.Position;

public class Setup {
    public static void setupNormal(Board normal){
        for(int i = 0; i < 8; i++)
            normal.addPiece(new Position(i, 6), new Pawn(Piece.Color.White));

        for(int i = 0; i < 8; i++)
            normal.addPiece(new Position(i, 1), new Pawn(Piece.Color.Black));

        normal.addPiece(new Position(4, 7), new King(Piece.Color.White));
        normal.addPiece(new Position(4, 0), new King(Piece.Color.Black));

        normal.addPiece(new Position(3, 7), new Queen(Piece.Color.White));
        normal.addPiece(new Position(3, 0), new Queen(Piece.Color.Black));

        normal.addPiece(new Position(1, 7), new Knight(Piece.Color.White));
        normal.addPiece(new Position(1, 0), new Knight(Piece.Color.Black));
        normal.addPiece(new Position(6, 7), new Knight(Piece.Color.White));
        normal.addPiece(new Position(6, 0), new Knight(Piece.Color.Black));

        normal.addPiece(new Position(0, 7), new Rook(Piece.Color.White));
        normal.addPiece(new Position(0, 0), new Rook(Piece.Color.Black));
        normal.addPiece(new Position(7, 7), new Rook(Piece.Color.White));
        normal.addPiece(new Position(7, 0), new Rook(Piece.Color.Black));

        normal.addPiece(new Position(2, 7), new Bishop(Piece.Color.White));
        normal.addPiece(new Position(2, 0), new Bishop(Piece.Color.Black));
        normal.addPiece(new Position(5, 7), new Bishop(Piece.Color.White));
        normal.addPiece(new Position(5, 0), new Bishop(Piece.Color.Black));

        normal.addPiece(new Position(0, 3), new Coin());
        normal.addPiece(new Position(7, 4), new Coin());
    }

    public static void setupHell(Board hell){
        hell.addPiece(new Position(0, 7), new Coin());
        hell.addPiece(new Position(5, 7), new Coin());
        hell.addPiece(new Position(1, 2), new Coin());
        hell.addPiece(new Position(3, 0), new Coin());
        hell.addPiece(new Position(7, 0), new Coin());

        hell.addPiece(new Position(2, 5), new Portal());

        hell.addPiece(new Position(4, 2, Board.BoardType.Hell), new Devil(0));
    }

    public static void setupHeaven(Board heaven){
        heaven.addPiece(new Position(7, 0), new Coin());

        heaven.addPiece(new Position(0, 7), new Portal());

        heaven.addPiece(new Position(1, 1, Board.BoardType.Heaven), new Angel(0));
        heaven.addPiece(new Position(5, 2, Board.BoardType.Heaven), new Atheism(0));
        heaven.addPiece(new Position(6, 5, Board.BoardType.Heaven), new Church(0));
    }
}
