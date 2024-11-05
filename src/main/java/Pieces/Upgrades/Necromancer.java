package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Bishop;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Necromancer extends Bishop {
    public Necromancer(Color color) {
        super(color);
    }

    @Override
    public void capturePiece(Piece piece, Position to, Position from) {
        if(piece == null) return;

        Main.game.getBoard(piece.getPosition().getLocation()).removePiece(piece.getPosition());

        Position respawnAt = from.difference(to);
        respawnAt = respawnAt.divide(respawnAt.absolute());
        respawnAt = to.add(respawnAt);

        piece.setColor(Main.game.isWhiteToMove() ? Color.White : Color.Black);

        Main.game.getCurrentBoard().addPiece(respawnAt, piece);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/necromancer.png");
        return Icons.icons.get("black/necromancer.png");
    }
}
