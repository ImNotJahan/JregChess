package main.java.Pieces.Shop;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Bishop;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Jester extends Bishop {
    public Jester(Color color) {
        super(color);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/jester.png");
        return Icons.icons.get("black/jester.png");
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.giveGold(1);

        if(position.getLocation() == Board.BoardType.Normal) {
            if(Main.game.heaven != null) {
                position.changeBoard(Board.BoardType.Heaven);
                Main.game.heaven.addPiece(getPosition(), this);
            }
        }

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
