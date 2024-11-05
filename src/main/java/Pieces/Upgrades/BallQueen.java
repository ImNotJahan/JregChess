package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.Queen;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class BallQueen extends Queen {
    public BallQueen(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        if(super.validMoveP(to, board)) return true;

        Position diff = position.difference(to);

        if(diff.absolute().getY() == 3 && diff.absolute().getX() <= 1) return true;
        if(diff.absolute().getX() == 3 && diff.absolute().getY() <= 1) return true;
        if(diff.absolute().getX() == 2 && diff.absolute().getY() == 2) return true;

        return false;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/ball-queen.png");
        return Icons.icons.get("black/ball-queen.png");
    }
}
