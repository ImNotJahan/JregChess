package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.Bishop;
import main.java.Pieces.Rook;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class BishopKnight extends Bishop {
    public BishopKnight(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        return super.validMoveP(to, board) ||
                (Math.abs(diff.getX()) == 2 && Math.abs(diff.getY()) == 1) ||
                (Math.abs(diff.getX()) == 1 && Math.abs(diff.getY()) == 2);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/bishop-knight.png");
        return Icons.icons.get("black/bishop-knight.png");
    }
}
