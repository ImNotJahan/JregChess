package main.java.Pieces.NPCs.Rules;

import main.java.Main;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class Treasure extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("treasure.png");
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.giveGold(15);
        return true;
    }
}
