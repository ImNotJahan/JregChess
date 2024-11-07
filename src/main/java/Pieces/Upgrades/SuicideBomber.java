package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Pawn;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class SuicideBomber extends Pawn {
    public SuicideBomber(Color color) {
        super(color);
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

        Main.game.getCurrentBoard().explode(position, this);

        if(killer != null)
            Main.game.normal.removePiece(killer.getPosition());

        return false;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/suicide-bomber.png");
        return Icons.icons.get("black/suicide-bomber.png");
    }
}
