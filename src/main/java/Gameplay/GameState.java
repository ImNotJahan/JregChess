package main.java.Gameplay;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.Piece;
import main.java.UI.BoardGUI;
import main.java.Util.IDs;
import main.java.Util.Position;

import javax.swing.*;
import java.util.ArrayList;

public class GameState {
    public final ArrayList<Rule> rules = new ArrayList<>();

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

        rules.add(Rule.KING_DIES_IN_HELL);
        rules.add(Rule.SUICIDE_BOMBER_HEAVEN);
    }

    public Board getBoard(Board.BoardType boardType){
        return switch (boardType){
            case Heaven -> heaven;
            case Hell -> hell;
            case Normal -> normal;
        };
    }

    public Board getCurrentBoard(){
        return getBoard(currentBoard);
    }

    private Position lastClicked;

    private boolean whiteToMove = true;

    public Position getLastClicked(){
        return lastClicked;
    }

    public boolean isWhiteToMove(){
        return whiteToMove;
    }

    private String[] upgradingFrom;
    private String upgradingTo;

    private enum Action { Nothing, Moving, Buying, Upgrading };
    private Action action = Action.Nothing;

    public void nowUpgrading(String[] from, String to){
        upgradingFrom = from;
        upgradingTo = to;
        action = Action.Upgrading;
    }

    private int itemBeingBoughtCost;
    private String itemBeingBoughtId;

    public void nowBuying(String id, int cost) {
        action = Action.Buying;
        itemBeingBoughtCost = cost;
        itemBeingBoughtId = id;
    }

    public boolean isUpgrading(){
        return action == Action.Upgrading;
    }

    public boolean isBuying(){
        return action == Action.Buying;
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

    private int funds(){
        if(whiteToMove) return whiteGP;
        return blackGP;
    }

    private void handleUpgrading(int x, int y){
        Board board = getCurrentBoard();
        Position pos = new Position(x, y);

        action = Action.Nothing;

        if(whiteToMove && whiteGP < 5) return;
        if(!whiteToMove && blackGP < 5) return;
        if(!board.pieceAt(pos)) return;
        if(!upgradingFromThisP(board.getPieceAt(pos).getClass().getSimpleName())) return;
        if(whiteToMove && board.getPieceAt(pos).getColor() != Piece.Color.White) return;
        if(!whiteToMove && board.getPieceAt(pos).getColor() != Piece.Color.Black) return;

        if(whiteToMove){
            whiteGP -= 5;
            board.removePiece(pos);
            board.addPiece(pos, IDs.pieceFromId(upgradingTo));
        } else{
            blackGP -= 5;
            board.removePiece(pos);

            Piece piece = IDs.pieceFromId(upgradingTo);
            piece.setColor(Piece.Color.Black);

            board.addPiece(pos, piece);
        }
    }

    private void handleBuying(int x, int y){
        action = Action.Nothing;

        Board board = getCurrentBoard();
        Position pos = new Position(x, y);

        if(currentBoard != Board.BoardType.Normal) return;
        if(funds() < itemBeingBoughtCost) return;
        if(board.pieceAt(pos)) return;

        withdraw(itemBeingBoughtCost);

        Piece piece = IDs.pieceFromId(itemBeingBoughtId);

        if(!whiteToMove) piece.setColor(Piece.Color.Black);

        board.addPiece(pos, piece);
    }

    public void withdraw(int amount){
        giveGold(-amount);
    }

    public void giveGold(int amount){
        if(whiteToMove) whiteGP += amount;
        else blackGP += amount;
    }

    public void handleClick(int x, int y){
        if(action == Action.Upgrading) {
            handleUpgrading(x, y);
            return;
        } else if(action == Action.Buying){
            handleBuying(x, y);
            return;
        }

        Board board = getCurrentBoard();
        Position pos = new Position(x, y);

        if(action == Action.Moving){
            if(board.movePiece(pos, lastClicked, true))
                whiteToMove = !whiteToMove;
            action = Action.Nothing;
            lastClicked = null;
        } else {
            if(board.pieceAt(pos)){
                if(board.getPieceAt(pos).getColor() == Piece.Color.White && !whiteToMove) return;
                if(board.getPieceAt(pos).getColor() == Piece.Color.Black && whiteToMove) return;
                if(board.getPieceAt(pos).getColor() == Piece.Color.NPC) return;

                action = Action.Moving;
                lastClicked = new Position(x, y);
            }
        }
    }
}
