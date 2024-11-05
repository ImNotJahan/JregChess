package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.Knight;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Unicorn extends Knight {
    public Unicorn(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        if(super.validMoveP(to, board)) return true;

        Position diff = position.difference(to);

        return (Math.abs(diff.getY()) == 2 && diff.getX() == 0) ||
                (Math.abs(diff.getX()) == 2 && diff.getY() == 0);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/unicorn.png");
        return Icons.icons.get("black/unicorn.png");
    }
}
