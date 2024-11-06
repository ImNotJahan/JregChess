package main.java.Pieces;

import main.java.Main;
import main.java.Util.Icons;
import main.java.Util.Position;
import main.java.Board.Board;

import javax.swing.*;

public class Pawn extends Piece {
    protected  boolean moved = false;

    public Pawn(Color color) {
        super(color);
    }

    @Override
    public void handleMove(Position to, Position from, Board board) {
        moved = true;
    }

    @Override
    public void moveTo(Position to) {
        super.moveTo(to);

        if(to.getY() == 0 && getColor() == Color.White ||
                to.getY() == 7 && getColor() == Color.Black) {
            Main.game.getCurrentBoard().removePiece(to);
            Main.game.getCurrentBoard().setPieceAt(to, new Queen(getColor()));
        }
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        if(getColor() == Color.Black) diff.flipY();

        if(!board.pieceAt(to) || board.getPieceAt(to).getColor() == Color.NPC){
            if(diff.getX() != 0) return false;
            if(diff.getY() > 2) return false;
            if(diff.getY() <= 0) return false;
            if(diff.getY() == 2 && moved) return false;
            if(diff.getY() == 2) {
                if (getColor() == Color.White) {
                    return !board.pieceAt(to.add(0, 1));
                } else {
                    return !board.pieceAt(to.add(0, -1));
                }
            }
        } else {
            if(diff.getY() != 1) return false;
            if(diff.getX() != 1 && diff.getX() != -1) return false;
        }

        return true;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/pawn.png");
        return Icons.icons.get("black/pawn.png");
    }
}
