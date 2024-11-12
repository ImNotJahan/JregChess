package main.java.Util;

import main.java.Main;

import javax.swing.*;
import java.util.HashMap;

public class Icons {
    public static HashMap<String, ImageIcon> icons = new HashMap<>();

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
            addNewIcon("bomb.png");
            addNewIcon("landmine.png");
            addNewIcon("gun.png");
            addNewIcon("meteor.png");
            addNewIcon("pittrap.png");
            addNewIcon("potion-seller.png");
            addNewIcon("treasure.png");
            addNewIcon("void.png");
            addNewIcon("upgrade-icon.png");
            addNewIcon("whirlpool.png");
            addNewIcon("wildlife.png");
            addNewIcon("zombie.png");
            addNewIcon("wild-horse.png");
            addNewIcon("explosion.png");
            addNewIcon("winds.png");

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
            addNewIcon("white/zebra.png");
            addNewIcon("white/giraffe.png");
            addNewIcon("white/jester.png");

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
            addNewIcon("black/zebra.png");
            addNewIcon("black/giraffe.png");
            addNewIcon("black/jester.png");

            addNewIcon("white/super-king0.png");
            addNewIcon("white/super-king1.png");
            addNewIcon("white/super-king2.png");
            addNewIcon("white/super-king3.png");

            addNewIcon("angel0.png");
            addNewIcon("angel1.png");
            addNewIcon("angel2.png");
            addNewIcon("angel3.png");
            addNewIcon("angel4.png");
            addNewIcon("angel5.png");
            addNewIcon("angel6.png");
            addNewIcon("angel7.png");
            addNewIcon("angel8.png");

            addNewIcon("devil.png");
            addNewIcon("devil0.png");
            addNewIcon("devil1.png");
            addNewIcon("devil2.png");
            addNewIcon("devil3.png");

            addNewIcon("hell.png");

            addNewIcon("atheism.png");
            addNewIcon("atheism0.png");
            addNewIcon("atheism1.png");
            addNewIcon("atheism2.png");
            addNewIcon("atheism3.png");

            addNewIcon("church0.png");
            addNewIcon("church1.png");
            addNewIcon("church2.png");
            addNewIcon("church3.png");
            addNewIcon("church4.png");
            addNewIcon("church5.png");

            addNewIcon("void0.png");
            addNewIcon("void1.png");
            addNewIcon("void2.png");
            addNewIcon("void3.png");

            addNewIcon("aggro-angel.png");
            addNewIcon("aggro-angel0.png");
            addNewIcon("aggro-angel1.png");
            addNewIcon("aggro-angel2.png");
            addNewIcon("aggro-angel3.png");
        } catch(Exception e){
            System.out.println(e);
        }
    }

    private static void addNewIcon(String filename) throws Exception {
        icons.put(filename, new ImageIcon(Main.resources.loadBufferedImage(filename)));
    }
}
