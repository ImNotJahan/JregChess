package main.java.Pieces.Shop;

import main.java.Board.Board;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Giraffe extends Piece {
    public Giraffe(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        return (Math.abs(diff.getX()) == 3 && Math.abs(diff.getY()) == 1) ||
                (Math.abs(diff.getX()) == 1 && Math.abs(diff.getY()) == 3);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/giraffe.png");
        return Icons.icons.get("black/giraffe.png");
    }
}
