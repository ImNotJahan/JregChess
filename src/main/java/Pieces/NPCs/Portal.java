package main.java.Pieces.NPCs;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;

public class Portal extends NPC {
    Board.BoardType to = null;

    public Portal(){
        super();
    }

    public Portal(Board.BoardType to){
        this.to = to;
    }

    @Override
    public Icon getIcon() {
        return Icons.icons.get("portal.png");
    }

    // portals move pieces between heaven and hell
    @Override
    public boolean kill(Piece killer) {
        Main.game.getCurrentBoard().removePiece(killer.getPosition());

        if(to != null){
            if(to == Board.BoardType.Heaven)
                Main.game.heaven.addPiece(position, killer);
            else
                Main.game.hell.addPiece(position, killer);
        }

        switch(Main.game.currentBoard){
            case Normal:
            case Hell:
                if(Main.game.heaven != null) Main.game.heaven.addPiece(position, killer);
                break;

            case Heaven:
                if(Main.game.hell != null) Main.game.hell.addPiece(position, killer);
                break;
        }

        return false;
    }
}
