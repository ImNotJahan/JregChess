package main.java.Pieces;

import main.java.Board.Board;
import main.java.Main;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public abstract class LargePiece extends Piece {
    protected final int id;

    protected LargePiece[] relatedPieces;

    public LargePiece(int id){
        this.id = id;
    }

    @Override
    public void moveTo(Position to) {
        if(id != 0) return;

        Board board = Main.game.getCurrentBoard();

        for(int i = 1; i < relatedPieces.length; i++){
            Position diff = new Position(i % getWidth(), Math.floorDiv(i, getWidth()));
            board.movePiece(to.add(diff), relatedPieces[i].getPosition());
        }
    }

    public void setPosition(Position position){
        this.position = position;
    }

    public abstract int getWidth();

    public abstract int getHeight();

    public int getId() {
        return id;
    }

    public LargePiece[] getRelatedPieces() {
        return relatedPieces;
    }

    public void spawnFullPiece() {
        relatedPieces = new LargePiece[getHeight() * getWidth()];

        for(int y = 0; y < getHeight(); y++){
            for(int x = 0; x < getWidth(); x++){
                if(x + y == 0) {
                    relatedPieces[0] = this;
                    continue;
                }

                LargePiece piece = spawnPiece(x + y * getWidth());

                relatedPieces[y * getWidth() + x] = piece;
                Main.game.getCurrentBoard().addPiece(position.add(x, y), piece);
            }
        }
    }

    protected abstract LargePiece spawnPiece(int id);

    protected abstract String getIconBase();

    @Override
    public Icon getIcon() {
        return Icons.icons.get(getIconBase() + id + ".png");
    }
}