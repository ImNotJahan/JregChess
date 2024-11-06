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
        Main.game.getCurrentBoard().takePiece(to, this);
    }

    @Override
    public boolean kill(Piece killer) {
        // blow up pieces in 1-tile radius around piece
        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0) continue;

                Main.game.normal.takePiece(position.add(x, y), this);
            }
        }

        return true;
    }
}
