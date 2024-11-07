package main.java.Pieces.NPCs.Shop;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class Landmine extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("landmine.png");
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.normal.explode(position, this);
        if(killer != null)
            Main.game.normal.removePiece(killer.getPosition());
        return false;
    }
}
