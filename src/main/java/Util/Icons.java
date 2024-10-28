package main.java.Util;

import main.java.Main;

import javax.swing.*;
import java.util.HashMap;

public class Icons {
    public static HashMap<String, Icon> icons = new HashMap<>();

    public static void init(){
        try {
            addNewIcon("white/pawn.png");
            addNewIcon("black/pawn.png");
            addNewIcon("white/suicide-bomber.png");
            addNewIcon("black/suicide-bomber.png");
            addNewIcon("white/king.png");
            addNewIcon("black/king.png");
            addNewIcon("white/queen.png");
            addNewIcon("black/queen.png");
            addNewIcon("white/rook.png");
            addNewIcon("black/rook.png");
            addNewIcon("white/bishop.png");
            addNewIcon("black/bishop.png");
            addNewIcon("white/knight.png");
            addNewIcon("black/knight.png");
            addNewIcon("coin.png");
            addNewIcon("portal.png");
            addNewIcon("placeholder.png");

            addNewIcon("white/angry-rook.png");
            addNewIcon("white/ball-queen.png");
            addNewIcon("white/bishop-knight.png");
            addNewIcon("white/centaur.png");
            addNewIcon("white/knight-queen.png");
            addNewIcon("white/necromancer.png");
            addNewIcon("white/rook-knight.png");
            addNewIcon("white/rook-tower.png");
            addNewIcon("white/super-king.png");
            addNewIcon("white/trojan-horse.png");
            addNewIcon("white/unicorn.png");
            addNewIcon("white/super-bishop.png");

            addNewIcon("black/angry-rook.png");
            addNewIcon("black/ball-queen.png");
            addNewIcon("black/bishop-knight.png");
            addNewIcon("black/centaur.png");
            addNewIcon("black/knight-queen.png");
            addNewIcon("black/necromancer.png");
            addNewIcon("black/rook-knight.png");
            addNewIcon("black/rook-tower.png");
            addNewIcon("black/super-king.png");
            addNewIcon("black/trojan-horse.png");
            addNewIcon("black/unicorn.png");
            addNewIcon("black/super-bishop.png");
        } catch(Exception e){
            System.out.println(e);
        }
    }

    private static void addNewIcon(String filename) throws Exception {
        icons.put(filename, new ImageIcon(Main.resources.loadBufferedImage(filename)));
    }
}
