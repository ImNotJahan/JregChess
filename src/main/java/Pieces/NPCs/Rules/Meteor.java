package main.java.Pieces.NPCs.Rules;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class Meteor extends NPC {
    @Override
    public boolean validMoveP(Position to, Board board) {
        return true;
    }

    @Override
    public void automove(Board board) {
        board.movePiece(position.add(1, 1), position);
    }

    @Override
    public void capturePiece(Piece piece, Position to, Position from) {
        if(piece == null) return;
        Main.game.normal.explode(position, this);
    }

    @Override
    public Icon getIcon() {
        return Icons.icons.get("meteor.png");
    }
}
