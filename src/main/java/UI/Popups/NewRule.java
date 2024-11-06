package main.java.UI.Popups;

import main.java.Gameplay.Rule;
import main.java.Main;
import main.java.UI.Components.RuleButton;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;

public class NewRule extends JFrame {
    private final RuleButton[] rules = new RuleButton[1];
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
        rules[0] = new RuleButton(Rule.LANDMINES);

        for(RuleButton rule : rules) {
            rule.addMouseListener(closeThis);
            add(rule);
        }
    }
}
