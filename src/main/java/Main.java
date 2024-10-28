package main.java;

import main.java.Gameplay.GameState;
import main.java.Util.Icons;
import main.java.UI.MainMenuUI;
import main.java.Util.Resources;

import javax.swing.SwingUtilities;

public class Main {
    public static GameState game;

    public static final Resources resources = new Resources();

    public static void main(String[] args) {
        Icons.init();

        SwingUtilities.invokeLater(MainMenuUI::new);
    }

    public static void startSingleplayerGame(){
        new GameState();
    }
}