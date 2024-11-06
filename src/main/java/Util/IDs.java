package main.java.Util;

import main.java.Pieces.NPCs.Placeholder;
import main.java.Pieces.*;
import main.java.Pieces.Shop.Zebra;
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
            case "super-king" -> new SuperKing(0, Piece.Color.White);
            case "necromancer" -> new Necromancer(Piece.Color.White);
            case "super-bishop" -> new SuperBishop(Piece.Color.White);
            case "ball-queen" -> new BallQueen(Piece.Color.White);
            case "centaur" -> new Centaur(Piece.Color.White);
            case "unicorn" -> new Unicorn(Piece.Color.White);
            case "trojan-horse" -> new TrojanHorse(Piece.Color.White);
            case "rook-tower" -> new RookTower(Piece.Color.White);
            case "zebra" -> new Zebra(Piece.Color.White);
            default -> new Placeholder();
        };
    }
}
