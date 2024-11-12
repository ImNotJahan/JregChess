package main.java.UI;

import main.java.Board.Board;
import main.java.Gameplay.GameState;
import main.java.Pieces.Piece;
import main.java.UI.Components.SwitchBoardButton;
import main.java.UI.Components.TileComponent;
import main.java.Util.Position;

import javax.swing.*;
import javax.swing.event.MouseInputAdapter;
import java.awt.*;
import java.awt.event.*;

public class BoardGUI extends JFrame {
    private final TileComponent[][] board = new TileComponent[8][8];
    private final GeneralUI generalUI;

    private final GameState game;

    public BoardGUI(GameState game) {
        setTitle("White to move");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLayout(new GridLayout(8, 8));

        this.game = game;

        initBoard();

        pack();
        setLocationRelativeTo(null);
        setVisible(true);

        addComponentListener(new ComponentAdapter() {
            @Override
            public void componentResized(ComponentEvent e) {
                setSize(new Dimension(getWidth(), getHeight()));
            }
        });

        generalUI = new GeneralUI(game);
        SwitchBoardButton.gui = this;
    }

    private void initBoard(){
        for(int y = 0; y < 8; y++){
            for(int x = 0; x < 8; x++){
                TileComponent tile = new TileComponent(x, y, game);

                final int xf = x;
                final int yf = y;

                MouseInputAdapter adapter = new MouseInputAdapter() {
                    @Override
                    public void mouseReleased(MouseEvent e) {
                        handleClick(xf, yf);
                    }
                };

                tile.addMouseListener(adapter);
                add(tile);
                board[y][x] = tile;
            }
        }

        redraw();
    }

    public void redraw(){
        drawBoard();
        if(generalUI != null) generalUI.redraw();
    }

    public void drawBoard(){
        Board boardObj = game.getCurrentBoard();

        for(int y = 0; y < 8; y++){
            for(int x = 0; x < 8; x++){
                Position pos = new Position(x, y);
                Piece piece = boardObj.getPieceAt(pos);
                board[y][x].setPiece(piece);

                if(game.isUpgrading() && piece != null) {
                    if(game.upgradingFromThisP(piece.getClass().getSimpleName()))
                        board[y][x].highlight((piece.getColor() == Piece.Color.White && game.isWhiteToMove()) ||
                                (piece.getColor() == Piece.Color.Black && !game.isWhiteToMove()), game);
                    else
                        board[y][x].highlight(false, game);
                } else if(game.isBuying()){
                    board[y][x].highlight(game.currentBoard == Board.BoardType.Normal && !boardObj.pieceAt(pos), game);
                }
                else {
                    board[y][x].highlight(boardObj.validMove(pos, game.getLastClicked(), true), game);
                    board[y][x].setSelected(pos.equals(game.getLastClicked()));
                }
            }
        }
    }

    public void handleClick(int x, int y) {
        game.handleClick(x, y);

        redraw();
    }
}
