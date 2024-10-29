package main.java.Board;

import main.java.Pieces.Piece;
import main.java.Util.Position;

public class Board {
    public enum BoardType { Normal, Heaven, Hell };
    private final Piece[][] pieces = new Piece[8][8];

    public boolean addPiece(Position at, Piece piece){
        if(pieceAt(at)) return false;

        return setPieceAt(at, piece);
    }

    // board makes sure no taking of our own pieces and on board
    public boolean movePiece(Position to, Position from){
        if(!validMove(to, from)) return false;

        takePiece(to);

        Piece piece = getPieceAt(from);
        setPieceAt(to, piece);

        piece.handleMove(to, from, this);

        removePiece(from);

        return true;
    }

    public boolean validMove(Position to, Position from) {
        if(from == null || to == null) return false;

        Piece piece = getPieceAt(from);

        if(piece == null) return false;
        if(!validLocation(to)) return false;
        if(pieceAt(to) && getPieceAt(to).getColor() == piece.getColor()) return false;
        if(!piece.validMoveP(to, this)) return false;
        if(to.equals(from)) return false;

        return true;
    }

    public void removePiece(Position pos){
        if(!validLocation(pos)) return;

        pieces[pos.getY()][pos.getX()] = null;
    }

    public void takePiece(Position pos){
        if(!validLocation(pos)) return;

        Piece piece = getPieceAt(pos);

        if(piece == null) return;

        removePiece(pos);
        piece.kill();
    }

    public boolean pieceAt(Position pos){
        return getPieceAt(pos) != null;
    }

    public boolean setPieceAt(Position pos, Piece piece){
        if(!validLocation(pos)) return false;
        if(piece == null) return false;

        pieces[pos.getY()][pos.getX()] = piece;
        piece.moveTo(pos);

        return true;
    }

    public Piece getPieceAt(Position pos){
        if(!validLocation(pos)) return null;

        return pieces[pos.getY()][pos.getX()];
    }

    public boolean validLocation(Position pos){
        if(pos.getX() >= 8 || pos.getX() < 0) return false;
        if(pos.getY() >= 8 || pos.getY() < 0) return false;

        return true;
    }
}
