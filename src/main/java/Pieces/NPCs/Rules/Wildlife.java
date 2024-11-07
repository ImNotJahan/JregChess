package main.java.Pieces.NPCs.Rules;

import main.java.Board.Board;
import main.java.Pieces.NPCs.NPC;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Wildlife extends NPC {
    private boolean movingRight;

    public Wildlife(boolean movingRight){
        this.movingRight = movingRight;
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return !board.pieceAt(to);
    }

    @Override
    public void automove(Board board) {
        board.movePiece(position.add(movingRight ? 1 : -1, 0), position);
    }

    @Override
    public void handleMove(Position to, Position from, Board board) {
        super.handleMove(to, from, board);

        if(to.getX() == 7) movingRight = false;
        else if(to.getX() == 0) movingRight = true;
    }

    @Override
    public Icon getIcon() {
        return Icons.icons.get("wildlife.png");
    }
}
