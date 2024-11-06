package main.java.UI.Components;

import main.java.Board.Board;
import main.java.Main;
import main.java.UI.BoardGUI;

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
                switch (Main.game.currentBoard){
                    case Normal:
                        if(Main.game.hell != null) Main.game.currentBoard = Board.BoardType.Hell;
                        else if(Main.game.heaven != null) Main.game.currentBoard = Board.BoardType.Heaven;
                        break;

                    case Hell:
                        if(Main.game.heaven != null) Main.game.currentBoard = Board.BoardType.Heaven;
                        else Main.game.currentBoard = Board.BoardType.Normal;
                        break;

                    case Heaven:
                        Main.game.currentBoard = Board.BoardType.Normal;
                        break;
                };

                gui.redraw();
            }
        });
    }
}
