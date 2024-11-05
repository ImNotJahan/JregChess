package main.java.Pieces.NPCs;

import main.java.Main;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class Coin extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("coin.png");
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.giveGold(4);
        return true;
    }
}
