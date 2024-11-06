package main.java.Pieces;

import main.java.Board.Board;
import main.java.Main;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class King extends Piece {
    public King(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        if(Math.abs(diff.getX()) > 1) return false;
        if(Math.abs(diff.getY()) > 1) return false;

        return true;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/king.png");
        return Icons.icons.get("black/king.png");
    }
}
