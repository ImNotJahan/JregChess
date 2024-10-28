package main.java.Pieces;

import main.java.Board.Board;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Rook extends Piece {
    public Rook(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        if(diff.getX() != 0 && diff.getY() != 0) return false;

        if(diff.getX() != 0){
            if(diff.getX() > 0){
                for(int i = 1; i < diff.getX(); i++){
                    if(board.pieceAt(position.add(-i, 0))) return false;
                }
            } else {
                for(int i = -1; i > diff.getX(); i--){
                    if(board.pieceAt(position.add(-i, 0))) return false;
                }
            }
        } else {
            if(diff.getY() > 0){
                for(int i = 1; i < diff.getY(); i++){
                    if(board.pieceAt(position.add(0, -i))) return false;
                }
            } else {
                for(int i = -1; i > diff.getY(); i--){
                    if(board.pieceAt(position.add(0, -i))) return false;
                }
            }
        }

        return true;
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/rook.png");
        return Icons.icons.get("black/rook.png");
    }
}
