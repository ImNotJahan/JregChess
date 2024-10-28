package main.java.Gameplay;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Piece;
import main.java.UI.BoardGUI;
import main.java.UI.SkillTree;
import main.java.Util.Position;

import javax.swing.*;

public class GameState {
    public final Board normal = new Board();
    public final Board heaven = new Board();
    public final Board hell = new Board();

    public int whiteGP = 5;
    public int blackGP = 5;

    public BoardGUI gui;

    public Board.BoardType currentBoard = Board.BoardType.Normal;

    public GameState(){
        init();
    }

    private void init() {
        Main.game = this;

        Setup.setupNormal(normal);
        Setup.setupHell(hell);
        Setup.setupHeaven(heaven);

        gui = new BoardGUI(this);
    }

    public Board getCurrentBoard(){
        return switch (currentBoard){
            case Heaven -> heaven;
            case Hell -> hell;
            case Normal -> normal;
        };
    }

    private boolean pieceSelected = false;
    private Position lastClicked;

    private boolean whiteToMove = true;

    public Position getLastClicked(){
        return lastClicked;
    }

    public boolean isWhiteToMove(){
        return whiteToMove;
    }

    private boolean upgrading = false;
    private String[] upgradingFrom;
    private String upgradingTo;

    public void nowUpgrading(String[] from, String to){
        upgradingFrom = from;
        upgradingTo = to;
        upgrading = true;
    }

    public boolean isUpgrading(){
        return upgrading;
    }

    public boolean upgradingFromThisP(String simpleClassName){
        for(String from : upgradingFrom){
            if(from.equals(simpleClassName)) return true;
        }

        return false;
    }

    public void gameWon(){
        if(whiteToMove)
            JOptionPane.showMessageDialog(gui, "White wins!");
        else
            JOptionPane.showMessageDialog(gui, "Black wins!");
    }

    private void handleUpgrading(int x, int y){
        Board board = getCurrentBoard();
        Position pos = new Position(x, y);

        upgrading = false;

        if(whiteToMove && whiteGP < 5) return;
        if(!whiteToMove && blackGP < 5) return;
        if(!board.pieceAt(pos)) return;
        if(!upgradingFromThisP(board.getPieceAt(pos).getClass().getSimpleName())) return;
        if(whiteToMove && board.getPieceAt(pos).getColor() != Piece.Color.White) return;
        if(!whiteToMove && board.getPieceAt(pos).getColor() != Piece.Color.Black) return;

        if(whiteToMove){
            whiteGP -= 5;
            board.removePiece(pos);
            board.addPiece(pos, SkillTree.pieceFromId(upgradingTo));
        } else{
            blackGP -= 5;
            board.removePiece(pos);

            Piece piece = SkillTree.pieceFromId(upgradingTo);
            piece.setColor(Piece.Color.Black);

            board.addPiece(pos, piece);
        }
    }

    public void giveGold(int amount){
        if(whiteToMove) whiteGP += amount;
        else blackGP += amount;
    }

    public void handleClick(int x, int y){
        if(upgrading) {
            handleUpgrading(x, y);
            return;
        }

        Board board = getCurrentBoard();
        Position pos = new Position(x, y);

        if(pieceSelected){
            if(board.movePiece(pos, lastClicked))
                whiteToMove = !whiteToMove;
            pieceSelected = false;
            lastClicked = null;
        } else {
            if(board.pieceAt(pos)){
                if(board.getPieceAt(pos).getColor() == Piece.Color.White && !whiteToMove) return;
                if(board.getPieceAt(pos).getColor() == Piece.Color.Black && whiteToMove) return;
                if(board.getPieceAt(pos).getColor() == Piece.Color.NPC) return;

                pieceSelected = true;
                lastClicked = new Position(x, y);
            }
        }
    }
}
