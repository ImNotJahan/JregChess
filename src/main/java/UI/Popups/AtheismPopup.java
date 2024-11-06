package main.java.UI.Popups;

import main.java.Board.Board;
import main.java.Gameplay.GameState;
import main.java.Main;
import main.java.Pieces.NPCs.Angel;
import main.java.Pieces.NPCs.Devil;
import main.java.Pieces.Piece;
import main.java.Util.Icons;
import main.java.Util.Position;

import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;

public class AtheismPopup extends JFrame {
    private final JPanel optionsPanel = new JPanel();
    private final MouseListener closeThis;

    public AtheismPopup(GameState game) {
        setTitle("God Of Atheism");
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
        add(new JLabel(Icons.icons.get("atheism.png")));

        optionsPanel.setLayout(new BoxLayout(optionsPanel, BoxLayout.Y_AXIS));

        JButton destroyHeavenButton = new JButton("DESTROY HEAVEN");
        JButton destroyHellButton = new JButton("DESTROY HELL");
        JButton destroyMetaphysicalButton = new JButton("DESTROY ALL METAPHYSICAL PIECES ON THE MATERIAL PLANE");

        destroyHeavenButton.addMouseListener(closeThis);
        destroyHellButton.addMouseListener(closeThis);
        destroyMetaphysicalButton.addMouseListener(closeThis);

        destroyHeavenButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.heaven = null;

                if(Main.game.currentBoard == Board.BoardType.Heaven) {
                    Main.game.currentBoard = Board.BoardType.Normal;
                    Main.game.gui.redraw();
                }
            }
        });

        destroyHellButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.hell = null;

                if(Main.game.currentBoard == Board.BoardType.Hell) {
                    Main.game.currentBoard = Board.BoardType.Normal;
                    Main.game.gui.redraw();
                }
            }
        });

        destroyMetaphysicalButton.addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Board board = Main.game.normal;

                for(int y = 0; y < 8; y++){
                    for(int x = 0; x < 8; x++){
                        Position pos = new Position(x, y);
                        Piece piece = board.getPieceAt(pos);

                        if(piece == null) continue;

                        if(piece instanceof Angel || piece instanceof Devil) {
                            board.takePiece(pos, null);
                        }
                    }
                }
            }
        });

        optionsPanel.add(destroyHeavenButton);
        optionsPanel.add(destroyHellButton);
        optionsPanel.add(destroyMetaphysicalButton);

        add(optionsPanel);
    }
}
