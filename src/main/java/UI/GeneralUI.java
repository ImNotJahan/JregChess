package main.java.UI;

import main.java.Gameplay.GameState;
import main.java.UI.Components.MoneyLabel;
import main.java.UI.Components.SwitchBoardButton;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class GeneralUI extends JFrame {
    private MoneyLabel moneyLabel;
    public SwitchBoardButton switchBoardButton;
    private final GameState game;

    public GeneralUI(GameState game) {
        setTitle("Toolbox");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLayout(new FlowLayout());

        this.game = game;

        init();

        pack();
        setVisible(true);
        setResizable(false);
    }

    private void init(){
        moneyLabel = new MoneyLabel();
        add(moneyLabel);

        JButton shopButton = new JButton("Shop");
        shopButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                new Shop();
            }
        });

        JButton skillTreeButton = new JButton("Skill tree");
        skillTreeButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                new SkillTree();
            }
        });

        add(shopButton);
        add(skillTreeButton);

        switchBoardButton = new SwitchBoardButton();
        add(switchBoardButton);

        redraw();
    }

    public void redraw(){
        moneyLabel.update(game);
        pack();
    }
}
