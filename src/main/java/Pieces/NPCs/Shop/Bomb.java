package main.java.Pieces.NPCs.Shop;

import main.java.Main;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Bomb extends NPC {
    @Override
    public Icon getIcon() {
        return Icons.icons.get("bomb.png");
    }

    @Override
    public void moveTo(Position to) {
        super.moveTo(to);
        Main.game.getCurrentBoard().takePiece(to, null);
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.normal.explode(position, this);
        return true;
    }
}
