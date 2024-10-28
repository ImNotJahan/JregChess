package main.java.Pieces.NPCs;

import main.java.Util.Icons;

import javax.swing.*;

public class Portal extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("portal.png");
    }

    @Override
    public void kill() {

    }
}
