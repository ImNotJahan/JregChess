package main.java.UI.Popups;

import main.java.Gameplay.GameState;
import main.java.Main;
import main.java.Pieces.NPCs.AggroDevil;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;

public class DevilPopup extends JFrame {
    private final JPanel optionsPanel = new JPanel();
    private final MouseListener closeThis;

    public DevilPopup(GameState game) {
        setTitle("The Devil");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new FlowLayout());

        closeThis = new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.clearAction();

                setVisible(false);
                dispose();
            }
        };

        init();

        pack();
        setVisible(true);
        setResizable(false);
    }

    private void init(){
        add(new JLabel(Icons.icons.get("devil.png")));

        optionsPanel.setLayout(new BoxLayout(optionsPanel, BoxLayout.Y_AXIS));

        JButton releaseButton = new JButton("RELEASE ME");
        JButton removeRulesButton = new JButton("REMOVE ANY RULE");
        JButton destroyGridButton = new JButton("SMITE ANY PIECE");
        JButton getGoldButton = new JButton("YOU GET 10 GOLD. YOUR OPPONENT GETS 5");

        releaseButton.addMouseListener(closeThis);
        removeRulesButton.addMouseListener(closeThis);
        destroyGridButton.addMouseListener(closeThis);
        getGoldButton.addMouseListener(closeThis);

        releaseButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Piece devil = new AggroDevil(0);
                Main.game.automovingPieces.add(devil);
                Main.game.normal.addPiece(new Position(3, 3), devil);

                Main.game.gui.redraw();
            }
        });

        getGoldButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                if(Main.game.isWhiteToMove()){
                    Main.game.whiteGP += 10;
                    Main.game.blackGP += 5;
                } else {
                    Main.game.whiteGP += 5;
                    Main.game.blackGP += 10;
                }
            }
        });

        optionsPanel.add(releaseButton);
        optionsPanel.add(removeRulesButton);
        optionsPanel.add(destroyGridButton);
        optionsPanel.add(getGoldButton);

        add(optionsPanel);
    }
}
