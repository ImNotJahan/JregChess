package main.java.UI.Components;

import main.java.Gameplay.GameState;
import main.java.Pieces.Piece;
import main.java.Util.Icons;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ComponentAdapter;
import java.awt.event.ComponentEvent;

public class TileComponent extends JButton {
    private final int x;
    private final int y;

    private ImageIcon icon = null;

    public TileComponent(int x, int y, GameState game){
        this.x = x;
        this.y = y;

        init(game);

        addComponentListener(new ComponentAdapter() {
            @Override
            public void componentResized(ComponentEvent event) {
                resizeIcon();
            }
        });
    }

    private void resizeIcon(){
        try{
            if(icon == null) return;

            int size = Math.min(getHeight(), getWidth());

            Image image = icon.getImage().getScaledInstance(size, size, Image.SCALE_DEFAULT);
            setIcon(new ImageIcon(image));
        }
        catch (Exception e) {
            System.out.println(e);
        }
    }

    public void init(GameState game){
        setPreferredSize(new Dimension(64, 64));

        setBorderPainted(false);

        refreshColor(game);

        setHorizontalAlignment(SwingConstants.CENTER);
        setVerticalAlignment(SwingConstants.CENTER);
    }

    private static final Color NORMAL_A = new Color(255, 207, 159);
    private static final Color NORMAL_B = new Color(210, 140, 69);
    private static final Color HEAVEN_A = new Color(196, 237, 242);
    private static final Color HEAVEN_B = new Color(104, 210, 222);
    private static final Color HELL_A = new Color(255, 0, 0);
    private static final Color HELL_B = new Color(153, 0, 0);

    private void refreshColor(GameState game){
        if ((x + y) % 2 == 0) {
            setBackground(
                    switch (game.currentBoard){
                        case Normal -> NORMAL_A;
                        case Heaven -> HEAVEN_A;
                        case Hell -> HELL_A;
                    }
            );
        } else {
            setBackground(
                    switch (game.currentBoard){
                        case Normal -> NORMAL_B;
                        case Heaven -> HEAVEN_B;
                        case Hell -> HELL_B;
                    }
            );
        }
    }

    public void setSelected(boolean selected){
        if(selected) setBackground(Color.CYAN);
    }

    public void highlight(boolean p, GameState game){
        if(p) setBackground(Color.GREEN);
        else refreshColor(game);
    }

    @Override
    public void setIcon(Icon defaultIcon) {
        super.setIcon(defaultIcon);
        icon = (ImageIcon) defaultIcon;
    }

    public void setPiece(Piece piece){
        if(piece == null) setIcon(null);
        else {
            setIcon(piece.getIcon());
            resizeIcon();
        }
    }
}
