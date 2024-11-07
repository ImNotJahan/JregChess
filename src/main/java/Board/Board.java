package main.java.Board;

import main.java.Gameplay.Rule;
import main.java.Main;
import main.java.Pieces.LargePiece;
import main.java.Pieces.Piece;
import main.java.Util.Position;

public class Board {
    public enum BoardType { Normal, Heaven, Hell };
    private final Piece[][] pieces = new Piece[8][8];
    private final BoardType type;

    public Board(BoardType type){
        this.type = type;
    }

    public boolean addPiece(Position at, Piece piece){
        if(pieceAt(at))
            if(!takePiece(at, piece)) return false;

        if(piece instanceof LargePiece && ((LargePiece) piece).getId() == 0) {
            // so the piece stays on the board
            if(at.getY() == 9 - ((LargePiece) piece).getWidth())
                at = at.add(0, -(((LargePiece) piece).getWidth() - 1));
            if(at.getY() == 9 - ((LargePiece) piece).getHeight())
                at = at.add(0, -(((LargePiece) piece).getHeight() - 1));

            ((LargePiece) piece).setPosition(at);
            ((LargePiece) piece).spawnFullPiece();
        }

        at.setLocation(type);

        return setPieceAt(at, piece);
    }

    // board makes sure no taking of our own pieces and on board
    public boolean movePiece(Position to, Position from, boolean wasClicked){
        if(!validMove(to, from, wasClicked)) return false;

        Piece piece = getPieceAt(from);

        if(wasClicked && piece instanceof LargePiece) {
            Position diff = ((LargePiece) piece).getDiff(((LargePiece) piece).getId());
            return movePiece(to.difference(diff), from.difference(diff));
        }

        Piece capturedPiece = getPieceAt(to);
        if(!takePiece(to, piece)) return true;

        setPieceAt(to, piece);

        piece.handleMove(to, from, this);

        removePiece(from);

        piece.capturePiece(capturedPiece, to, from);

        return true;
    }

    public void explode(Position position, Piece taker) {
        removePiece(position);

        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0) continue;

                takePiece(position.add(x, y), taker);
            }
        }
    }

    public boolean movePiece(Position to, Position from){
        return movePiece(to, from, false);
    }

    public boolean validMove(Position to, Position from){
        return validMove(to, from, false);
    }

    public boolean validMove(Position to, Position from, boolean wasClicked) {
        if(from == null || to == null) return false;

        Piece piece = getPieceAt(from);

        if(piece == null) return false;
        if(!validLocation(to)) return false;
        if(pieceAt(to) && getPieceAt(to).getColor() == piece.getColor()) return false;
        if(!piece.validMoveP(to, this)) return false;
        if(to.equals(from)) return false;

        // make sure that other parts of large piece don't make illegal move
        if(wasClicked && piece instanceof LargePiece){
            Position diff = ((LargePiece) piece).getDiff();
            Position originTo = to.difference(diff);
            Position originFrom = from.difference(diff);

            for(int y = 0; y < ((LargePiece) piece).getHeight(); y++){
                for(int x = 0; x < ((LargePiece) piece).getWidth(); x++){
                    if(!validMove(originTo.add(x, y), originFrom.add(x, y))) return false;
                }
            }
        }

        return true;
    }

    public void removePiece(Position pos){
        if(!validLocation(pos)) return;

        pieces[pos.getY()][pos.getX()] = null;
    }

    public boolean takePiece(Position pos, Piece taker){
        if(!validLocation(pos)) return false;

        Piece piece = getPieceAt(pos);

        if(piece == null) return true;

        if(piece instanceof LargePiece && ((LargePiece) piece).getId() != 0)
            return takePiece(pos.difference(((LargePiece) piece).getDiff()), taker);

        if(!piece.kill(taker)) return false;

        // if the leader piece, kill the related pieces
        if(piece instanceof LargePiece && ((LargePiece) piece).getId() == 0) {
            for(int y = 0; y < ((LargePiece) piece).getHeight(); y++) {
                for (int x = 0; x < ((LargePiece) piece).getWidth(); x++) {
                    if (x + y == 0) continue;
                    removePiece(pos.add(x, y));
                }
            }
        }

        removePiece(pos);

        if(Main.game.rules.remove(Rule.NEXT_PIECE_EXPLODES)){
            explode(pos, piece);
            return false;
        }

        return true;
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
