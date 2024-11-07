package main.java.Pieces.NPCs.Shop;

import main.java.Board.Board;
import main.java.Gameplay.RulesAndEvents;
import main.java.Main;
import main.java.Pieces.LargePiece;
import main.java.Pieces.Piece;
import main.java.Util.Position;

public class Void extends LargePiece {
    public Void(int id) {
        super(id);
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
        return new Void(id);
    }

    @Override
    protected String getIconBase() {
        return "void";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }

    @Override
    public Color getColor() {
        return Color.NPC;
    }

    @Override
    public boolean kill(Piece killer) {
        if(killer != null) {
            boolean moved = false;

            Main.game.normal.removePiece(killer.getPosition());

            while(!moved){
                Position pos = new Position(RulesAndEvents.randInt(0, 7), RulesAndEvents.randInt(0, 7));

                if(Main.game.normal.pieceAt(pos)) continue;

                Main.game.normal.setPieceAt(pos, killer);
                moved = true;
            }
        }

        return false;
    }
}
