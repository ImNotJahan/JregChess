package main.java.Util;

import main.java.Pieces.NPCs.Placeholder;
import main.java.Pieces.*;
import main.java.Pieces.Upgrades.*;

public class IDs {
    public static Piece pieceFromId(String id){
        return switch (id){
            case "pawn" -> new Pawn(Piece.Color.White);
            case "rook" -> new Rook(Piece.Color.White);
            case "knight" -> new Knight(Piece.Color.White);
            case "bishop" -> new Bishop(Piece.Color.White);
            case "king" -> new King(Piece.Color.White);
            case "queen" -> new Queen(Piece.Color.White);
            case "suicide-bomber" -> new SuicideBomber(Piece.Color.White);
            case "knight-queen" -> new KnightQueen(Piece.Color.White);
            case "bishop-knight" -> new BishopKnight(Piece.Color.White);
            case "rook-knight" -> new RookKnight(Piece.Color.White);
            case "angry-rook" -> new AngryRook(Piece.Color.White);
            default -> new Placeholder();
        };
    }
}
