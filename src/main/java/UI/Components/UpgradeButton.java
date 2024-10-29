package main.java.UI.Components;

import main.java.Main;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class UpgradeButton extends JButton {
    private final String[] from;

    public UpgradeButton(String id, String... upgradeeClassNames){
        setIcon(Icons.icons.get("white/" + id + ".png"));
        from = upgradeeClassNames;

        addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.nowUpgrading(from, id);

                Main.game.gui.redraw();
            }
        });
    }
}
