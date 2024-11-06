package main.java.UI.Components;

import main.java.Gameplay.Rule;
import main.java.Main;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class RuleButton extends JButton {
    public RuleButton(Rule rule){
        setIcon(Icons.icons.get(getIcon(rule) + ".png"));

        addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.addRule(rule);
            }
        });
    }

    private String getIcon(Rule rule){
        return switch (rule){
            case NEXT_PIECE_EXPLODES -> "white/suicide-bomber";
            case PAWNS_MOVE_FOUR -> "white/pawn";
            case BISHOPS_GAIN_NECROMANCY -> "white/necromancer";
            //case ZOMBIE_APOCALYPSE -> "";
            //case GUN -> "";
            //case WILD_LIFE -> "";
            //case WILD_HORSE -> "";
            //case TREADMILL_BOARD -> "";
            case MEGA_CASTLE -> "white/king";
            //case WHIRLPOOL -> "";
            case LANDMINES -> "landmine";
            //case VOID -> "";
            //case PITTRAPS -> "";

            default -> "placeholder";
        };
    }
}
