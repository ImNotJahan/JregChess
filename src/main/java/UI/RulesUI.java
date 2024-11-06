package main.java.UI;

import main.java.Gameplay.GameState;
import main.java.Gameplay.Rule;
import main.java.Main;
import main.java.UI.Popups.NewRule;

import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;

public class RulesUI extends JFrame {
    JPanel panel = new JPanel();

    public RulesUI(GameState game) {
        setTitle("Rules");
        setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE);
        setLayout(new FlowLayout());
        setMinimumSize(new Dimension(200, 100));

        init();

        setVisible(true);
        setResizable(false);
    }

    private void init(){
        panel.setBorder(new EmptyBorder(10, 10, 10, 10));
        panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));
        add(panel);

        redraw();
    }

    public void redraw() {
        panel.removeAll();

        for(Rule rule : Main.game.rules)
            panel.add(new JLabel(ruleToString(rule)));

        pack();
    }

    private static String ruleToString(Rule rule){
        return switch (rule){
            case CENTAUR_MAKING -> "You can move pawns onto horses to make centaurs";
            case KING_DIES_IN_HELL -> "King must die in hell";
            case SUICIDE_BOMBER_HEAVEN -> "Suicide bombers go to heaven";
            case NEXT_PIECE_EXPLODES -> "NEXT PIECE TAKEN EXPLODES";
            case PAWNS_MOVE_FOUR -> "PAWNS CAN MOVE UP TO FOUR SPACES ON THEIR FIRST TURN";
            case BISHOPS_GAIN_NECROMANCY -> "BISHOPS GAIN NECROMANCY";
            case ZOMBIE_APOCALYPSE -> "";
            case GUN -> "";
            case WILD_LIFE -> "";
            case WILD_HORSE -> "";
            case TREADMILL_BOARD -> "";
            case MEGA_CASTLE -> "";
            case WHIRLPOOL -> "";
            case LANDMINES -> "";
            case VOID -> "";
            case PITTRAPS -> "";

            default -> "Unknown rule";
        };
    }
}
