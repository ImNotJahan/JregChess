package main.java;

import main.java.Gameplay.GameState;
import main.java.Networking.API;
import main.java.UI.Popups.Waiting;
import main.java.Util.Icons;
import main.java.UI.MainMenuUI;
import main.java.Util.Resources;

import javax.swing.*;
import java.util.Random;
import java.util.concurrent.TimeUnit;

public class Main {
    public static GameState game;
    public static String seed;
    public static Random random = new Random();

    public static final Resources resources = new Resources();

    public static void main(String[] args) {
        Icons.init();

        SwingUtilities.invokeLater(MainMenuUI::new);
    }

    public static void startSingleplayerGame(){
        new GameState();
    }

    public static void startMultiplayerGame() {
        generateSeed();

        API.newGame(seed);

        new GameState(true, true);
        game.dialog("Join code is " + seed);
    }

    public static void joinMultiplayerGame(String seed) {
        Main.seed = seed;
        generateRandom();

        new GameState(true, false);
        waitForTurn();
    }

    private static void generateRandom(){
        long longSeed = 0;

        for(char c : seed.toCharArray()){
            longSeed = 31L * longSeed + c;
        }

        Main.random = new Random(longSeed);
    }

    private static void generateSeed(){
        Random random = new Random();
        // 97 to 122 is the range of lowercase letters
        seed = random.ints(97, 122 + 1)
                .limit(5).collect(StringBuilder::new, StringBuilder::appendCodePoint, StringBuilder::append)
                .toString();

        generateRandom();
    }

    public static boolean stillLoading = false;

    public static void waitForTurn(){
        game.nowHandlingPopup();

        final Waiting waiting = new Waiting();

        new java.util.Timer().schedule(
            new java.util.TimerTask() {
                @Override
                public void run() {
                    if(stillLoading) return;

                    stillLoading = true;
                    if(API.getTurn(seed) == game.playingAsWhite){
                        cancel();
                        game.clearAction();
                        cancel();
                        waiting.close();
                        game.handleTurn(API.getMove(Main.seed));
                    }
                    stillLoading = false;
                }
            }, 0, 200
        );
    }
}