package main.java.UI.Popups;

import main.java.Gameplay.Rule;
import main.java.UI.Components.RuleButton;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;

public class NewRule extends JFrame {
    private final RuleButton[] rules = new RuleButton[21];
    private final MouseListener closeThis;

    public NewRule(){
        setTitle("Pick a rule");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new GridLayout(5, 5));

        closeThis = new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                setVisible(false);
                dispose();
            }
        };

        init();

        pack();
        setVisible(true);
        setResizable(false);
    }

    private void init(){
        rules[0] = new RuleButton(Rule.EVERYONE_UPGRADES);
        rules[1] = new RuleButton(Rule.PAWNS_MOVE_FOUR);
        rules[2] = new RuleButton(Rule.BISHOPS_GAIN_NECROMANCY);
        rules[3] = new RuleButton(Rule.GOLD_RUSH);
        rules[4] = new RuleButton(Rule.METEOR_SHOWER);
        rules[5] = new RuleButton(Rule.ZOMBIE_APOCALYPSE);
        rules[6] = new RuleButton(Rule.WILD_LIFE);
        rules[7] = new RuleButton(Rule.GUN);
        rules[8] = new RuleButton(Rule.WILD_HORSE);
        rules[9] = new RuleButton(Rule.NEXT_PIECE_EXPLODES);
        rules[10] = new RuleButton(Rule.UNICORNS);
        rules[11] = new RuleButton(Rule.PORTALS_OPEN);
        rules[12] = new RuleButton(Rule.PAWN_UPGRADE);
        rules[13] = new RuleButton(Rule.MEGA_CASTLE);
        rules[14] = new RuleButton(Rule.WHIRLPOOL);
        rules[15] = new RuleButton(Rule.LANDMINES);
        rules[16] = new RuleButton(Rule.MORE_GOLD);
        rules[17] = new RuleButton(Rule.VOID);
        rules[18] = new RuleButton(Rule.PITTRAPS);
        rules[19] = new RuleButton(Rule.TREASURE);
        rules[20] = new RuleButton(Rule.POTIONS);

        for(RuleButton rule : rules) {
            rule.addMouseListener(closeThis);
            add(rule);
        }
    }
}
