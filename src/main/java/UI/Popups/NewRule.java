package main.java.UI.Popups;

import main.java.Gameplay.Rule;
import main.java.UI.Components.RuleButton;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.util.ArrayList;

public class NewRule extends JFrame {
    public static final ArrayList<RuleButton> rules = new ArrayList<>();
    private final MouseListener closeThis;

    public NewRule(){
        setTitle("Pick a rule");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new GridLayout(6, 3));

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
        if(rules.isEmpty()) {
            rules.add(new RuleButton(Rule.EVERYONE_UPGRADES));
            rules.add(new RuleButton(Rule.PAWNS_MOVE_FOUR));
            rules.add(new RuleButton(Rule.BISHOPS_GAIN_NECROMANCY));
            rules.add(new RuleButton(Rule.GOLD_RUSH));
            rules.add(new RuleButton(Rule.METEOR_SHOWER));
            rules.add(new RuleButton(Rule.ZOMBIE_APOCALYPSE));
            rules.add(new RuleButton(Rule.WILD_LIFE));
            //rules.add(new RuleButton(Rule.GUN));
            rules.add(new RuleButton(Rule.WILD_HORSE));
            rules.add(new RuleButton(Rule.NEXT_PIECE_EXPLODES));
            rules.add(new RuleButton(Rule.UNICORNS));
            rules.add(new RuleButton(Rule.PORTALS_OPEN));
            rules.add(new RuleButton(Rule.PAWN_UPGRADE));
            //rules.add(new RuleButton(Rule.MEGA_CASTLE));
            rules.add(new RuleButton(Rule.WHIRLPOOL));
            rules.add(new RuleButton(Rule.LANDMINES));
            rules.add(new RuleButton(Rule.MORE_GOLD));
            rules.add(new RuleButton(Rule.VOID));
            rules.add(new RuleButton(Rule.PITTRAPS));
            rules.add(new RuleButton(Rule.TREASURE));
            //rules.add(new RuleButton(Rule.POTIONS));
        }

        for(RuleButton rule : rules) {
            rule.addMouseListener(closeThis);
            add(rule);
        }
    }
}
