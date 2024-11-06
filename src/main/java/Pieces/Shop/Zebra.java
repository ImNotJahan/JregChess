package main.java.Pieces.Shop;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Knight;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Zebra extends Knight {
    public Zebra(Color color) {
        super(color);
    }

    // takes pieces it jumps over
    @Override
    public void capturePiece(Piece piece, Position to, Position from) {
        Position diff = to.difference(from);
        Position abs = diff.absolute();

        int multiplier;

        Board board = Main.game.getCurrentBoard();

        if(abs.getX() > abs.getY()) {
            multiplier = diff.getX() / abs.getX();

            for (int i = 1; i <= 2; i++) {
                board.takePiece(from.add(i * multiplier, 0), this);
            }
        } else {
            multiplier = diff.getY() / abs.getY();

            for (int i = 1; i <= 2; i++) {
                board.takePiece(from.add(0, i * multiplier), this);
            }
        }
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/zebra.png");
        return Icons.icons.get("black/zebra.png");
    }
}
