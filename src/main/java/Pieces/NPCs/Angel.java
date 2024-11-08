package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.LargePiece;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

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

    @Override
    public boolean kill(Piece killer) {
        if(JOptionPane.showConfirmDialog(Main.game.gui, Icons.icons.get("aggro-angel.png"), "Free him?", JOptionPane.YES_NO_OPTION) == JOptionPane.YES_OPTION){
            Piece piece = new AggroAngel(0);
            Main.game.normal.addPiece(new Position(3, 3), piece);
            Main.game.automovingPieces.add(piece);
            return true;
        }

        return false;
    }
}
