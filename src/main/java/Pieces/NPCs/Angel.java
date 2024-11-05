package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Pieces.LargePiece;
import main.java.Util.Position;

public class Angel extends LargePiece {
    public Angel(int id) {
        super(id);
        color = Color.NPC;
    }

    @Override
    public int getWidth() {
        return 3;
    }

    @Override
    public int getHeight() {
        return 3;
    }

    @Override
    protected LargePiece spawnPiece(int id) {
        return new Angel(id);
    }

    @Override
    protected String getIconBase() {
        return "angel";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }
}
