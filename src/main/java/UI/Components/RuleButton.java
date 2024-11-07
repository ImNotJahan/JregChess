package main.java.UI.Components;

import main.java.Gameplay.Rule;
import main.java.Main;
import main.java.UI.Popups.NewRule;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class RuleButton extends JButton {
    public RuleButton(Rule rule){
        setIcon(Icons.icons.get(getIcon(rule) + ".png"));

        RuleButton button = this;

        addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.addRule(rule);
                NewRule.rules.remove(button);
            }
        });
    }

    private String getIcon(Rule rule){
        return switch (rule){
            case NEXT_PIECE_EXPLODES -> "white/suicide-bomber";
            case PAWNS_MOVE_FOUR -> "white/pawn";
            case BISHOPS_GAIN_NECROMANCY -> "white/necromancer";
            case ZOMBIE_APOCALYPSE -> "zombie";
            case GUN -> "gun";
            case WILD_LIFE -> "wildlife";
            case WILD_HORSE -> "wild-horse";
            case MEGA_CASTLE -> "white/king";
            case WHIRLPOOL -> "whirlpool";
            case LANDMINES -> "landmine";
            case VOID -> "void";
            case PITTRAPS -> "pittrap";
            case GOLD_RUSH, MORE_GOLD -> "coin";
            case EVERYONE_UPGRADES -> "upgrade-icon";
            case METEOR_SHOWER -> "meteor";
            case UNICORNS -> "white/unicorn";
            case PORTALS_OPEN -> "portal";
            case PAWN_UPGRADE -> "black/pawn";
            case TREASURE -> "treasure";
            case POTIONS -> "potion-seller";

            default -> "placeholder";
        };
    }
}
