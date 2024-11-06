package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.LargePiece;
import main.java.UI.Healthbar;
import main.java.Util.Position;

public class SuperKing extends LargePiece {
    private int health = 2;
    private Healthbar healthbar;

    public SuperKing(int id, Color color) {
        super(id);
        this.color = color;

        if(id == 0)
            healthbar = new Healthbar((color == Color.White ? "White" : "Black") + " Super King", health);
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
        Position diff = position.difference(to);

        if(Math.abs(diff.getX()) > 2) return false;
        if(Math.abs(diff.getY()) > 2) return false;

        return true;
    }

    @Override
    public int getHealth() {
        return health;
    }

    @Override
    public void hurt() {
        health--;
        if(id == 0) healthbar.set(health);
    }
}
