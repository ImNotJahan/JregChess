package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Pieces.LargePiece;
import main.java.Util.Position;

public class Church extends LargePiece {
    public Church(int id) {
        super(id);
        color = Color.NPC;
    }

    @Override
    public int getWidth() {
        return 2;
    }

    @Override
    public int getHeight() {
        return 3;
    }

    @Override
    protected LargePiece spawnPiece(int id) {
        return new Church(id);
    }

    @Override
    protected String getIconBase() {
        return "church";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }
}
