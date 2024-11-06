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
