package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Gameplay.RulesAndEvents;
import main.java.Main;
import main.java.Pieces.LargePiece;
import main.java.Pieces.Piece;
import main.java.UI.Healthbar;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class AggroDevil extends LargePiece {
    Healthbar healthbar;

    public AggroDevil(int id) {
        super(id);
        color = Color.NPC;

        if(id == 0)
            healthbar = new Healthbar("Devil", health);
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
        return new AggroDevil(id);
    }

    @Override
    protected String getIconBase() {
        return "devil";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }

    @Override
    public void automove(Board board) {

    }

    int health = 6;

    @Override
    public int getHealth() {
        return health;
    }

    @Override
    public void hurt() {
        health--;
        if(id == 0) healthbar.set(health);
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.normal.takePiece(killer.getPosition(), this);

        if(!super.kill(killer)) return false;

        if(id == 0)
            Main.game.automovingPieces.remove(this);

        return true;
    }
}
