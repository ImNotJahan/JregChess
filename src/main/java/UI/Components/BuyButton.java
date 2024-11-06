package main.java.UI.Components;

import main.java.Main;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class BuyButton extends JButton {
    public BuyButton(String id, int cost){
        this(id, cost, false);
    }

    public BuyButton(String id, int cost, boolean NPC){
        if(NPC) setIcon(Icons.icons.get(id + ".png"));
        else setIcon(Icons.icons.get("white/" + id + ".png"));

        setText(cost + "GP");

        addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.nowBuying(id, cost);

                Main.game.gui.redraw();
            }
        });
    }
}
