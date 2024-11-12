package main.java.Util;

import main.java.Pieces.NPCs.*;
import main.java.Pieces.*;
import main.java.Pieces.NPCs.Shop.*;
import main.java.Pieces.Shop.*;
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
            case "giraffe" -> new Giraffe(Piece.Color.White);
            case "landmine" -> new Landmine();
            case "bomb" -> new Bomb();
            case "jester" -> new Jester(Piece.Color.White);
            case "portal" -> new Portal();
            default -> new Placeholder();
        };
    }

    public static char idToChar(String id){
        return switch (id){
            case "pawn" -> 'a';
            case "rook" -> 'b';
            case "knight" -> 'c';
            case "bishop" -> 'd';
            case "king" -> 'e';
            case "queen" -> 'f';
            case "suicide-bomber" -> 'g';
            case "knight-queen" -> 'h';
            case "bishop-knight" -> 'i';
            case "rook-knight" -> 'j';
            case "angry-rook" -> 'k';
            case "super-king" -> 'l';
            case "necromancer" -> 'm';
            case "super-bishop" -> 'n';
            case "ball-queen" -> 'o';
            case "centaur" -> 'p';
            case "unicorn" -> 'q';
            case "trojan-horse" -> 'r';
            case "rook-tower" -> 's';
            case "zebra" -> 't';
            case "giraffe" -> 'u';
            case "landmine" -> 'v';
            case "bomb" -> 'w';
            case "jester" -> 'x';
            case "portal" -> 'y';
            case "Pawn" -> 'A';
            case "Bishop" -> 'B';
            case "King" -> 'C';
            case "Knight" -> 'D';
            case "Queen" -> 'E';
            case "Rook" -> 'F';
            default -> 'z';
        };
    }

    public static String charToId(char c){
        return switch(c){
            case 'a' -> "pawn";
            case 'b' -> "rook";
            case 'c' -> "knight";
            case 'd' -> "bishop";
            case 'e' -> "king";
            case 'f' -> "queen";
            case 'g' -> "suicide-bomber";
            case 'h' -> "knight-queen";
            case 'i' -> "bishop-knight";
            case 'j' -> "rook-knight";
            case 'k' -> "angry-rook";
            case 'l' -> "super-king";
            case 'm' -> "necromancer";
            case 'n' -> "super-bishop";
            case 'o' -> "ball-queen";
            case 'p' -> "centaur";
            case 'q' -> "unicorn";
            case 'r' -> "trojan-horse";
            case 's' -> "rook-tower";
            case 't' -> "zebra";
            case 'u' -> "giraffe";
            case 'v' -> "landmine";
            case 'w' -> "bomb";
            case 'x' -> "jester";
            case 'y' -> "portal";
            case 'A' -> "Pawn";
            case 'B' -> "Bishop";
            case 'C' -> "King";
            case 'D' -> "Knight";
            case 'E' -> "Queen";
            case 'F' -> "Rook";
            default -> "placeholder";
        };
    }
}
