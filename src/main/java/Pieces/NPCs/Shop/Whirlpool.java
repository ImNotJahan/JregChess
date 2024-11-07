package main.java.Pieces.NPCs.Shop;

import main.java.Main;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class Whirlpool extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("whirlpool.png");
    }

    @Override
    public boolean kill(Piece killer) {
        if(killer != null)
            Main.game.getCurrentBoard().removePiece(killer.getPosition());
        return false;
    }
}
