package main.java.UI;

import main.java.Main;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class MainMenuUI extends JFrame {
    public MainMenuUI() {
        setTitle("Chess");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLayout(new BoxLayout(getContentPane(), BoxLayout.Y_AXIS));

        init();

        pack();
        setLocationRelativeTo(null);
        setVisible(true);
    }

    private void init(){
        JButton singleplayerButton = new JButton("Offline game");
        JButton multiplayerButton = new JButton("Create online game");
        JButton joinMultiplayerButton = new JButton("Join online game");

        singleplayerButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.startSingleplayerGame();
                setVisible(false);
                dispose();
            }
        });

        multiplayerButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.startMultiplayerGame();
                setVisible(false);
                dispose();
            }
        });

        joinMultiplayerButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.joinMultiplayerGame(JOptionPane.showInputDialog("Enter join code:"));
                setVisible(false);
                dispose();
            }
        });

        add(singleplayerButton);
        add(multiplayerButton);
        add(joinMultiplayerButton);
    }
}
