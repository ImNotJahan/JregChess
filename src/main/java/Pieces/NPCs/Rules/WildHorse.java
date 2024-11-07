package main.java.Pieces.NPCs.Rules;

import main.java.Board.Board;
import main.java.Gameplay.RulesAndEvents;
import main.java.Pieces.NPCs.NPC;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;

public class WildHorse extends NPC {
    @Override
    public void automove(Board board) {
        int multiplier1 = RulesAndEvents.randInt(0, 1) == 0 ? 1 : -1;
        int multiplier2 = RulesAndEvents.randInt(0, 1) == 0 ? 1 : -1;

        if(RulesAndEvents.randInt(0, 1) == 0)
            board.movePiece(position.add(2 * multiplier1, multiplier2), position);
        else
            board.movePiece(position.add(multiplier1, 2 * multiplier2), position);
    }

    @Override
    public boolean validMoveP(Position to, Board board) {
        return true;
    }

    @Override
    public Icon getIcon() {
        return Icons.icons.get("wild-horse.png");
    }
}
