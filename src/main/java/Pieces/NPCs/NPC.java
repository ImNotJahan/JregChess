package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Pieces.Piece;
import main.java.Util.Position;

import javax.swing.*;

public class NPC extends Piece {
    public NPC(){
        super(Color.NPC);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }

    @Override
    public Icon getIcon() {
        return null;
    }

    @Override
    public boolean kill(Piece killer) {
        // NPCs don't get to go to the afterlife.
        return true;
    }
}
