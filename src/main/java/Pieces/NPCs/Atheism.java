package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Pieces.LargePiece;
import main.java.Util.Position;

public class Atheism extends LargePiece {
    public Atheism(int id) {
        super(id);
        color = Color.NPC;
    }

    @Override
    public int getWidth() {
        return 2;
    }

    @Override
    public int getHeight() {
        return 2;
    }

    @Override
    protected LargePiece spawnPiece(int id) {
        return new Atheism(id);
    }

    @Override
    protected String getIconBase() {
        return "atheism";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }
}
