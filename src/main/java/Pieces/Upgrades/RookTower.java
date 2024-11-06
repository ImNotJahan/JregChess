package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Pawn;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class RookTower extends Piece {
    public RookTower(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }

    @Override
    public boolean kill(Piece killer) {
        Board board = Main.game.getCurrentBoard();

        // switches the color of all pieces around it
        for(int y = -1; y <= 1; y++){
            for(int x = -1; x <= 1; x++){
                Position pos = getPosition().add(x, y);

                Piece piece = board.getPieceAt(pos);

                if(piece == null) continue;

                if(piece.getColor() == Color.White) piece.setColor(Color.Black);
                else if(piece.getColor() == Color.Black) piece.setColor(Color.White);
            }
        }

        return super.kill(killer);
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/rook-tower.png");
        return Icons.icons.get("black/rook-tower.png");
    }
}
