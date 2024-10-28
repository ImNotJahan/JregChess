package main.java.Pieces.NPCs;

import main.java.Main;
import main.java.Util.Icons;

import javax.swing.*;

public class Coin extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("coin.png");
    }

    @Override
    public void kill() {
        Main.game.giveGold(4);
    }
}
