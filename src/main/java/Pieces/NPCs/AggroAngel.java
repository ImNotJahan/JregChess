package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Gameplay.RulesAndEvents;
import main.java.Main;
import main.java.Pieces.LargePiece;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class AggroAngel extends LargePiece {
    public AggroAngel(int id) {
        super(id);
        color = Color.NPC;
    }

    @Override
    public int getWidth() {
        return 2;
    }

    @Override
    public int getHeight() {
        return 2;
    }

    @Override
    protected LargePiece spawnPiece(int id) {
        return new AggroAngel(id);
    }

    @Override
    protected String getIconBase() {
        return "aggro-angel";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }

    @Override
    public void automove(Board board) {
        JOptionPane.showMessageDialog(Main.game.gui, Icons.icons.get("winds.png"));

        switch(RulesAndEvents.randInt(1, 4)){
            case 1:
                // move right
                for(int y = 0; y < 8; y++){
                    for(int x = 6; x >= 0; x--){
                        Position pos = new Position(x, y);

                        if(board.pieceAt(pos.add(1, 0))) continue;

                        board.forceMovePiece(pos.add(1, 0), pos);
                    }
                }

                break;

            case 2:
                // move left
                for(int y = 0; y < 8; y++){
                    for(int x = 1; x < 8; x++){
                        Position pos = new Position(x, y);

                        if(board.pieceAt(pos.add(-1, 0))) continue;

                        board.forceMovePiece(pos.add(-1, 0), pos);
                    }
                }

                break;

            case 3:
                // move down
                for(int y = 6; y >= 0; y--){
                    for(int x = 0; x < 8; x++){
                        Position pos = new Position(x, y);

                        if(board.pieceAt(pos.add(0, 1))) continue;

                        board.forceMovePiece(pos.add(0, 1), pos);
                    }
                }

                break;

            case 4:
                // move up
                for(int y = 1; y < 8; y++){
                    for(int x = 0; x < 8; x++){
                        Position pos = new Position(x, y);

                        if(board.pieceAt(pos.add(0, -1))) continue;

                        board.forceMovePiece(pos.add(0, -1), pos);
                    }
                }

                break;
        }
    }

    @Override
    public boolean kill(Piece killer) {
        if(id == 0)
            Main.game.automovingPieces.remove(this);

        return true;
    }
}
