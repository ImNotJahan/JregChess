package main.java.Gameplay;

import main.java.Board.Board;
import main.java.Main;
import main.java.Networking.API;
import main.java.Pieces.Piece;
import main.java.UI.BoardGUI;
import main.java.UI.Components.SwitchBoardButton;
import main.java.UI.Popups.NewRule;
import main.java.Util.IDs;
import main.java.Util.Position;

import javax.swing.*;
import java.awt.*;
import java.util.ArrayList;

public class GameState {
    public StringBuilder currentMove = new StringBuilder();

    public final ArrayList<Rule> rules = new ArrayList<>();

    public final Board normal = new Board(Board.BoardType.Normal);
    public Board heaven = new Board(Board.BoardType.Heaven);
    public Board hell = new Board(Board.BoardType.Hell);

    public ArrayList<Piece> automovingPieces = new ArrayList<>();

    public int whiteGP = 5;
    public int blackGP = 5;

    public BoardGUI gui;

    public Board.BoardType currentBoard = Board.BoardType.Normal;

    private int turnsSinceNewRule = 0;

    public final boolean online;
    public final boolean playingAsWhite; // only relevant if online game

    public GameState(){
        this(false, true);
    }

    public GameState(boolean online, boolean playingAsWhite){
        this.online = online;
        this.playingAsWhite = playingAsWhite;

        if(online){
            dialog("You are playing as " + (playingAsWhite ? "white" : "black"));
        }

        init();
    }

    public void addRule(Rule rule){
        action = Action.Nothing;

        if(online && (whiteToMove == playingAsWhite)) {
            currentMove.append("R");
            currentMove.append(RulesAndEvents.ruleToChar(rule));
        }

        RulesAndEvents.handleNew(rule, this);

        if(!RulesAndEvents.isEvent(rule))
            rules.add(rule);
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

    private enum Action { Nothing, Moving, Buying, Upgrading, HandlingPopup };
    private Action action = Action.Nothing;

    public void nowUpgrading(String[] from, String to){
        upgradingFrom = from;
        upgradingTo = to;
        action = Action.Upgrading;

        if(online && (whiteToMove == playingAsWhite)) {
            currentMove.append("U");
            currentMove.append(IDs.idToChar(to));

            for(String id : from)
                currentMove.append(IDs.idToChar(id));

            currentMove.append('Z');
        }
    }

    public void nowHandlingPopup(){
        action = Action.HandlingPopup;
    }

    public void clearAction(){
        action = Action.Nothing;
    }

    private int itemBeingBoughtCost;
    private String itemBeingBoughtId;

    public void nowBuying(String id, int cost) {
        action = Action.Buying;
        itemBeingBoughtCost = cost;
        itemBeingBoughtId = id;

        if(online && (whiteToMove == playingAsWhite)) {
            currentMove.append("B");
            currentMove.append(IDs.idToChar(id));
            currentMove.append(itemBeingBoughtCost);
            currentMove.append('Z');
        }
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

    public void gameWon(Piece.Color loosingColor){
        if(loosingColor == Piece.Color.Black)
            dialog("White wins!");
        else
            dialog("Black wins!");
    }

    public void dialog(String message) {
        JOptionPane.showMessageDialog(gui, message);
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

    public void handleTurn(String move){
        for(int i = 0; i < move.length(); i+=2){
            if(move.charAt(i) == 'R'){
                turnsSinceNewRule = 0;
                addRule(RulesAndEvents.charToRule(move.charAt(i + 1)));
            } else if(move.charAt(i) == 'B'){
                final String buying = IDs.charToId(move.charAt(i + 1));
                int cost = 0;

                i++;

                while(move.charAt(i + 1) != 'Z'){
                    i++;

                    cost *= 10;
                    cost += ctoi(move.charAt(i));
                }

                nowBuying(buying, cost);
            } else if(move.charAt(i) == 'U'){
                final String to = IDs.charToId(move.charAt(i + 1));
                ArrayList<String> from = new ArrayList<>();

                i++;

                while(move.charAt(i + 1) != 'Z'){
                    i++;

                    from.add(IDs.charToId(move.charAt(i)));
                }

                nowUpgrading(from.toArray(new String[0]), to);
            } else if(move.charAt(i) == 'S') {
                SwitchBoardButton.switchBoard();
            }else {
                handleClick(ctoi(move.charAt(i)), ctoi(move.charAt(i + 1)));
            }
        }
    }

    private int ctoi(char c){
        return switch(c){
            case '0' -> 0;
            case '1' -> 1;
            case '2' -> 2;
            case '3' -> 3;
            case '4' -> 4;
            case '5' -> 5;
            case '6' -> 6;
            case '7' -> 7;
            case '8' -> 8;
            case '9' -> 9;
            default -> -1;
        };
    }

    public void nextTurn() {
        whiteToMove = !whiteToMove;
        turnsSinceNewRule++;

        gui.setTitle(whiteToMove ? "White to move" : "Black to move");

        for (int i = 0; i < automovingPieces.size(); i++) {
            // to avoid exception when modifing arraylist during run
            automovingPieces.get(i).automove(normal);
        }

        if(online && (whiteToMove == playingAsWhite)) {
            Taskbar.getTaskbar().requestWindowUserAttention(gui);
        } else if(online) {
            API.move(Main.seed, currentMove.toString());
            currentMove = new StringBuilder();

            Main.waitForTurn();

            return;
        }

        checkIfNewRule();
    }
    
    private void checkIfNewRule(){
        if(turnsSinceNewRule < rules.size() * 2) return;

        new NewRule();
        action = Action.HandlingPopup;
        turnsSinceNewRule = 0;
    }
    
    public void handleClick(int x, int y){
        if(online && whiteToMove == playingAsWhite){
            currentMove.append(x);
            currentMove.append(y);
        }

        if(action == Action.HandlingPopup) return;

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
            action = Action.Nothing;

            if(board.movePiece(pos, lastClicked, true))
                nextTurn();

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
