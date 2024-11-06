package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Pawn;
import main.java.Pieces.Piece;
import main.java.Pieces.Queen;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class TrojanHorse extends Piece {
    public TrojanHorse(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        if(getColor() == Color.Black) diff.flipY();

        return diff.getY() == 1 && diff.getX() == 0;
    }

    @Override
    public boolean kill(Piece killer) {
        int spawnedPawns = 0;

        Board board = Main.game.getCurrentBoard();

        for(int y = -1; y <= 1; y++){
            for(int x = -1; x <= 1; x++){
                Position pos = getPosition().add(x, y);

                if(board.pieceAt(pos)) continue;

                board.addPiece(pos, new Pawn(color));

                if(++spawnedPawns == 3) break;
            }

            if(spawnedPawns == 3) break;
        }

        return true;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/trojan-horse.png");
        return Icons.icons.get("black/trojan-horse.png");
    }
}
