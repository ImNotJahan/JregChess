package main.java.UI;

import main.java.Gameplay.GameState;
import main.java.Gameplay.Rule;
import main.java.Main;

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

    public static String ruleToString(Rule rule){
        return switch (rule){
            case CENTAUR_MAKING -> "You can move pawns onto horses to make centaurs";
            case KING_DIES_IN_HELL -> "King must die in hell";
            case SUICIDE_BOMBER_HEAVEN -> "Suicide bombers go to heaven";
            case NEXT_PIECE_EXPLODES -> "NEXT PIECE TAKEN EXPLODES";
            case PAWNS_MOVE_FOUR -> "PAWNS CAN MOVE UP TO FOUR SPACES ON THEIR FIRST TURN";
            case BISHOPS_GAIN_NECROMANCY -> "BISHOPS GAIN NECROMANCY";
            case ZOMBIE_APOCALYPSE -> "ZOMBIE APOCALYPSE";
            case GUN -> "GUN SPAWNS";
            case WILD_LIFE -> "SPAWN WILD LIFE";
            case WILD_HORSE -> "SPAWN WILD HORSE";
            case TREADMILL_BOARD -> "TREADMILL BOARD";
            case MEGA_CASTLE -> "KINGS CAN CASTLE WITH ANY PIECE";
            case WHIRLPOOL -> "WHIRLPOOLS APPEAR";
            case LANDMINES -> "LANDMINES SPAWN";
            case VOID -> "VOIDVOIDVOIDVOID";
            case PITTRAPS -> "PITTRAPS SPAWN";
            case EVERYONE_UPGRADES -> "EVERYONE UPGRADES A PIECE";
            case GOLD_RUSH -> "GOLD RUSH";
            case METEOR_SHOWER -> "METEOR SHOWER";
            case UNICORNS -> "KNIGHTS BECOME UNICORNS";
            case PORTALS_OPEN -> "PORTALS APPEAR";
            case PAWN_UPGRADE -> "PAWNS UPGRADE";
            case MORE_GOLD -> "EVERYONE GETS GOLD";
            case TREASURE -> "TREASURE APPEARS";
            case POTIONS -> "POTION SELLER ARRIVES";
            default -> "Unknown rule";
        };
    }
}
