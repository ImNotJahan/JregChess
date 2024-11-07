package main.java.Pieces.NPCs.Rules;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Zombie extends NPC {
    private boolean movingRight;

    public Zombie(boolean movingRight){
        this.movingRight = movingRight;
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return true;
    }

    @Override
    public void automove(Board board) {
        Position to = position.add(movingRight ? 1 : -1, 0);

        if(board.getPieceAt(to) instanceof Zombie){
            movingRight = !movingRight;

            to = position.add(movingRight ? 1 : -1, 0);
        }

        board.movePiece(to, position);

        if(to.getX() == 7) movingRight = false;
        else if(to.getX() == 0) movingRight = true;
    }

    @Override
    public void capturePiece(Piece piece, Position to, Position from) {
        if(piece == null) return;

        Zombie zombie = new Zombie(movingRight);
        Main.game.getCurrentBoard().addPiece(from, zombie);
        Main.game.automovingPieces.add(zombie);
    }

    @Override
    public Icon getIcon() {
        return Icons.icons.get("zombie.png");
    }
}
