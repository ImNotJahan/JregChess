package main.java.Util;

import main.java.Board.Board;

public class Position {
    private Board.BoardType location;

    private int x;
    private int y;

    public Position(int x, int y, Board.BoardType location){
        this.x = x;
        this.y = y;
        this.location = location;
    }

    public Position(int x, int y){
        this(x, y, Board.BoardType.Normal);
    }

    // Changes x and y to given vector
    public void moveTo(int x, int y){
        this.x = x;
        this.y = y;
    }

    // Changes x and y by given vector
    public void moveBy(int x, int y){
        this.x += x;
        this.y += y;
    }

    public void changeBoard(Board.BoardType board){
        location = board;
    }

    public int getX(){
        return x;
    }

    public int getY() {
        return y;
    }

    public Board.BoardType getLocation(){
        return location;
    }

    public Position difference(Position other){
        return new Position(getX() - other.getX(), getY() - other.getY());
    }

    public void flipY(){
        y = -y;
    }

    public Position add(int x, int y){
        return new Position(getX() + x, getY() + y);
    }

    public boolean equals(Position pos) {
        if(pos == null) return false;
        return x == pos.getX() && y == pos.getY();
    }

    @Override
    public String toString() {
        return "(" + x + ", " + y + ")";
    }
}
