package main.java.UI;

import main.java.Gameplay.GameState;

import javax.swing.*;
import java.awt.*;

public class RulesUI extends JFrame {
    public RulesUI(GameState game) {
        setTitle("Rules");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLayout(new FlowLayout());

        init();

        pack();
        setVisible(true);
        setResizable(false);
    }

    private void init(){
        add(new JLabel("Something something"));
    }
}
