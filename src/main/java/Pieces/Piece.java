package main.java.Pieces;

import main.java.Board.Board;
import main.java.Main;
import main.java.Util.Position;

import javax.swing.*;

public abstract class Piece {
    public enum Color { White, Black, NPC };

    protected Color color;
    protected Position position;

    protected void init() {

    }

    public Piece(Color color, Position position){
        this.position = position;
        this.color = color;

        init();
    }

    public Piece(Color color){
        this(color, new Position(0, 0));
    }

    public Piece(){
        this(Color.White);
    }

    public abstract boolean validMoveP(Position to, Board board);

    public void moveTo(Position to){
        position = to;
    }

    public Position getPosition(){
        return position;
    }

    public Color getColor(){
        return color;
    }

    public void setColor(Color color){
        this.color = color;
    }

    // If returns false, doesn't die
    public boolean kill(Piece killer){
        if(position.getLocation() == Board.BoardType.Normal) {
            position.changeBoard(Board.BoardType.Hell);
            Main.game.hell.addPiece(getPosition(), this);
            Main.game.giveGold(1);
        }

        return true;
    }

    public void handleMove(Position to, Position from, Board board){}

    public abstract Icon getIcon();
}
