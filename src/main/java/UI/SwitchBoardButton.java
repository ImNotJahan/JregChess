package main.java.UI;

import main.java.Board.Board;
import main.java.Main;

import javax.swing.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

public class SwitchBoardButton extends JButton {
    public BoardGUI gui;

    public SwitchBoardButton(){
        super();

        setText("Switch board");

        addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent e) {
                Main.game.currentBoard = switch (Main.game.currentBoard){
                    case Normal -> Board.BoardType.Hell;
                    case Hell -> Board.BoardType.Heaven;
                    case Heaven -> Board.BoardType.Normal;
                };

                gui.redraw();
            }
        });
    }
}
