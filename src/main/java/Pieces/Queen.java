package main.java.Pieces;

import main.java.Board.Board;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Queen extends Piece {
    public Queen(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        if(Math.abs(diff.getX()) == Math.abs(diff.getY())) {
            int modifier = diff.getY() > 0 ? 1 : -1;

            if (diff.getX() > 0) {
                for (int i = 1; i < diff.getX(); i++) {
                    if (board.pieceAt(position.add(-i, -i * modifier))) return false;
                }
            } else {
                for (int i = -1; i > diff.getX(); i--) {
                    if (board.pieceAt(position.add(-i, i * modifier))) return false;
                }
            }

            return true;
        }

        if(diff.getX() == 0 || diff.getY() == 0) {
            if (diff.getX() != 0) {
                if (diff.getX() > 0) {
                    for (int i = 1; i < diff.getX(); i++) {
                        if (board.pieceAt(position.add(-i, 0))) return false;
                    }
                } else {
                    for (int i = -1; i > diff.getX(); i--) {
                        if (board.pieceAt(position.add(-i, 0))) return false;
                    }
                }
            } else {
                if (diff.getY() > 0) {
                    for (int i = 1; i < diff.getY(); i++) {
                        if (board.pieceAt(position.add(0, -i))) return false;
                    }
                } else {
                    for (int i = -1; i > diff.getY(); i--) {
                        if (board.pieceAt(position.add(0, -i))) return false;
                    }
                }
            }

            return true;
        }

        return false;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/queen.png");
        return Icons.icons.get("black/queen.png");
    }
}
