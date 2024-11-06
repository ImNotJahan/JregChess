package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.LargePiece;
import main.java.Pieces.Piece;
import main.java.UI.Popups.AtheismPopup;
import main.java.UI.Popups.DevilPopup;
import main.java.Util.Position;

public class Atheism extends LargePiece {
    public Atheism(int id) {
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
        return new Atheism(id);
    }

    @Override
    protected String getIconBase() {
        return "atheism";
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return false;
    }

    @Override
    public boolean kill(Piece killer) {
        Main.game.nowHandlingPopup();
        new AtheismPopup(Main.game);

        return false;
    }
}
