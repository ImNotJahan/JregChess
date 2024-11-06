package main.java.UI.Popups;

import main.java.Gameplay.GameState;
import main.java.Main;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;

public class DevilPopup extends JFrame {
    private final JPanel optionsPanel = new JPanel();
    private final MouseListener closeThis;

    public DevilPopup(GameState game) {
        setTitle("The Devil");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new FlowLayout());

        closeThis = new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.clearAction();

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
        add(new JLabel(Icons.icons.get("devil.png")));

        optionsPanel.setLayout(new BoxLayout(optionsPanel, BoxLayout.Y_AXIS));

        JButton releaseButton = new JButton("RELEASE ME");
        JButton removeRulesButton = new JButton("REMOVE ANY 2 RULES");
        JButton destroyGridButton = new JButton("DESTROY A ROW OR COLUMN");
        JButton getGoldButton = new JButton("YOU GET 20 GOLD. YOUR OPPONENT GETS 10");

        releaseButton.addMouseListener(closeThis);
        removeRulesButton.addMouseListener(closeThis);
        destroyGridButton.addMouseListener(closeThis);
        getGoldButton.addMouseListener(closeThis);

        getGoldButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.giveGold(20);
            }
        });

        optionsPanel.add(releaseButton);
        optionsPanel.add(removeRulesButton);
        optionsPanel.add(destroyGridButton);
        optionsPanel.add(getGoldButton);

        add(optionsPanel);
    }
}
