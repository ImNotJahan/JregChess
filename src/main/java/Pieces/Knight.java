package main.java.Pieces;

import main.java.Board.Board;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Knight extends Piece {
    public Knight(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        return (Math.abs(diff.getX()) == 2 && Math.abs(diff.getY()) == 1) ||
                (Math.abs(diff.getX()) == 1 && Math.abs(diff.getY()) == 2);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/knight.png");
        return Icons.icons.get("black/knight.png");
    }
}
