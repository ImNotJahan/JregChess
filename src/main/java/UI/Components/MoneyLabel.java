package main.java.UI.Components;

import main.java.Gameplay.GameState;

import javax.swing.*;

public class MoneyLabel extends JLabel {
    public void update(GameState game){
        setText("<html><body>White GP: " + game.whiteGP + "<br>Black GP: " + game.blackGP + "</body></html>");
    }
}
