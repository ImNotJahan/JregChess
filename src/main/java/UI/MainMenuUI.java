package main.java.UI;

import main.java.Main;

import javax.swing.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class MainMenuUI extends JFrame {
    public MainMenuUI() {
        setTitle("Chess");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLayout(new BoxLayout(getContentPane(), BoxLayout.Y_AXIS));

        init();

        pack();
        setVisible(true);
    }

    private void init(){
        JButton singleplayerButton = new JButton("Singleplayer");
        JButton multiplayerButton = new JButton("Multiplayer");

        singleplayerButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.startSingleplayerGame();
                setVisible(false);
                dispose();
            }
        });

        add(singleplayerButton);
        add(multiplayerButton);
    }
}
