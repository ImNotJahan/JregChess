package main.java.Pieces.Upgrades;

import main.java.Board.Board;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class AngryRook extends Piece {
    public AngryRook(Color color) {
        super(color);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        Position diff = position.difference(to);

        int count = 0;

        if(diff.getX() != 0){
            if(diff.getX() > 0){
                for(int i = 1; i < diff.getX(); i++){
                    if(board.pieceAt(position.add(-i, 0))) count++;
                }
            } else {
                for(int i = -1; i > diff.getX(); i--){
                    if(board.pieceAt(position.add(-i, 0))) count++;
                }
            }
        } else {
            if(diff.getY() > 0){
                for(int i = 1; i < diff.getY(); i++){
                    if(board.pieceAt(position.add(0, -i))) count++;
                }
            } else {
                for(int i = -1; i > diff.getY(); i--){
                    if(board.pieceAt(position.add(0, -i))) count++;
                }
            }
        }

        if(count > 1) return false;

        return diff.getX() == 0 || diff.getY() == 0;
    }

    @Override
    public void moveTo(Position to) {
        super.moveTo(to);
    }

    @Override
    public void handleMove(Position to, Position from, Board board) {
        Position diff = from.difference(to);

        if(diff.getX() != 0){
            if(diff.getX() > 0){
                for(int i = 1; i < diff.getX(); i++){
                    Piece piece = board.getPieceAt(position.add(i, 0));

                    if(piece != null) board.takePiece(position.add(i, 0), this);
                }
            } else {
                for(int i = -1; i > diff.getX(); i--){
                    Piece piece = board.getPieceAt(position.add(i, 0));

                    if(piece != null) board.takePiece(position.add(i, 0), this);
                }
            }
        } else {
            if(diff.getY() > 0){
                for(int i = 1; i < diff.getY(); i++){
                    Piece piece = board.getPieceAt(position.add(0, i));

                    if(piece != null) board.takePiece(position.add(0, i), this);
                }
            } else {
                for(int i = -1; i > diff.getY(); i--){
                    Piece piece = board.getPieceAt(position.add(0, i));

                    if(piece != null) board.takePiece(position.add(0, i), this);
                }
            }
        }
    }

    @Override
    public Icon getIcon() {
        if(getColor() == Color.White) return Icons.icons.get("white/angry-rook.png");
        return Icons.icons.get("black/angry-rook.png");
    }
}
