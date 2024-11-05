package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.Bishop;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class SuperBishop extends Bishop {
    public SuperBishop(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        return super.validMoveP(to, board) ||
                (Math.abs(diff.getX()) <= 1 &&
                        Math.abs(diff.getY()) <= 1);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/super-bishop.png");
        return Icons.icons.get("black/super-bishop.png");
    }
}
