package main.java.UI.Popups;

import main.java.Gameplay.GameState;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.*;

public class DevilPopup extends JFrame {
    private JPanel optionsPanel = new JPanel();

    public DevilPopup(GameState game) {
        setTitle("The Devil");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new FlowLayout());
        //setContentPane(new JLabel(Icons.icons.get("hell.png")));

        init();

        pack();
        setVisible(true);
        setResizable(false);
    }

    private void init(){
        add(new JLabel(Icons.icons.get("devil.png")));

        optionsPanel.setLayout(new BoxLayout(optionsPanel, BoxLayout.Y_AXIS));
        optionsPanel.add(new JButton("RELEASE ME"));
        optionsPanel.add(new JButton("REMOVE ANY 2 RULES"));
        optionsPanel.add(new JButton("DESTROY A ROW OR COLUMN"));
        optionsPanel.add(new JButton("YOU GET 10 GOLD. YOUR OPONENT GETS 5"));

        add(optionsPanel);
    }
}
