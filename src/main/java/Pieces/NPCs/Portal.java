package main.java.Pieces.NPCs;

import main.java.Main;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class Portal extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("portal.png");
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.getCurrentBoard().removePiece(killer.getPosition());
        Main.game.normal.addPiece(position, killer);

        return false;
    }
}
