package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.LargePiece;
import main.java.Util.Position;

public class SuperKing extends LargePiece {

    public SuperKing(int id, Color color) {
        super(id);
        this.color = color;
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
        return new SuperKing(id, color);
    }

    @Override
    protected String getIconBase() {
        if(color == Color.White) return "white/super-king";
        return "black/super-king";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return true;
    }
}
