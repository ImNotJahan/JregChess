package main.java.Gameplay;

import main.java.Board.Board;
import main.java.Main;
import main.java.Pieces.*;
import main.java.Pieces.NPCs.Coin;
import main.java.Pieces.NPCs.NPC;
import main.java.Pieces.NPCs.Portal;
import main.java.Pieces.NPCs.Rules.*;
import main.java.Pieces.NPCs.Shop.Landmine;
import main.java.Pieces.NPCs.Shop.Pittrap;
import main.java.Pieces.NPCs.Shop.Void;
import main.java.Pieces.NPCs.Shop.Whirlpool;
import main.java.Pieces.Shop.Jester;
import main.java.Pieces.Upgrades.*;
import main.java.Util.Position;

public class RulesAndEvents {
    public static boolean isEvent(Rule rule){
        return switch(rule){
            case MORE_GOLD, EVERYONE_UPGRADES, POTIONS, BISHOPS_GAIN_NECROMANCY, CENTAUR_MAKING -> true;
            default -> false;
        };
    }

    public static void handleNew(Rule rule, GameState game){
        switch(rule) {
            case MORE_GOLD:
                game.dialog("EVERYONE GETS +10GP!");
                game.whiteGP += 10;
                game.blackGP += 10;
                break;

            case GOLD_RUSH:
                game.dialog("GOLD RUSH!");

                for(int i = 0; i < 5; i++){
                    Position pos = new Position(randInt(0, 7), randInt(0, 7));

                    if(game.normal.pieceAt(pos)) continue;

                    game.normal.addPiece(pos, new Coin());
                }

                game.gui.redraw();
                break;

            case UNICORNS:
                for(int y = 0; y < 8; y++){
                    for(int x = 0; x < 8; x++){
                        Position pos = new Position(x, y);
                        Piece piece = game.normal.getPieceAt(pos);

                        if(piece == null) continue;

                        if(piece instanceof Knight) {
                            game.normal.setPieceAt(pos, new Unicorn(piece.getColor()));
                        }
                    }
                }

                game.gui.redraw();
                break;

            case BISHOPS_GAIN_NECROMANCY:
                for(int y = 0; y < 8; y++){
                    for(int x = 0; x < 8; x++){
                        Position pos = new Position(x, y);
                        Piece piece = game.normal.getPieceAt(pos);

                        if(piece == null) continue;

                        if(piece instanceof Bishop) {
                            game.normal.setPieceAt(pos, new Necromancer(piece.getColor()));
                        }
                    }
                }

                game.gui.redraw();
                break;

            case PORTALS_OPEN:
                game.normal.addPiece(new Position(0, 4), new Portal(Board.BoardType.Heaven));
                game.normal.addPiece(new Position(7, 3), new Portal(Board.BoardType.Hell));

                game.gui.redraw();
                break;

            case PAWN_UPGRADE:
                boolean upgradedWhite = false;
                boolean upgradedBlack = false;

                for(int y = 0; y < 8; y++){
                    for(int x = 0; x < 8; x++){
                        Position pos = new Position(x, y);
                        Piece piece = game.normal.getPieceAt(pos);

                        if(piece == null) continue;

                        if(piece instanceof Pawn) {
                            if(piece.getColor() == Piece.Color.White && upgradedWhite) continue;
                            if(piece.getColor() == Piece.Color.Black && upgradedBlack) continue;

                            game.normal.setPieceAt(pos, new Centaur(piece.getColor()));

                            if(piece.getColor() == Piece.Color.White) upgradedWhite = true;
                            else upgradedBlack = true;
                        }
                    }
                }

                game.gui.redraw();
                break;

            case TREASURE:
                game.normal.addPiece(new Position(randInt(0, 7), randInt(3, 4)), new Treasure());
                game.gui.redraw();
                break;

            case LANDMINES:
                for(int i = 0; i < 3; i++){
                    Position pos = new Position(randInt(0, 7), randInt(0, 7));

                    if(game.normal.pieceAt(pos)) continue;

                    game.normal.addPiece(pos, new Landmine());
                }

                game.gui.redraw();
                break;

            case WILD_LIFE:
                NPC wildlife = new Wildlife(true);

                game.automovingPieces.add(wildlife);
                game.normal.addPiece(new Position(0, 3), wildlife);

                wildlife = new Wildlife(false);

                game.automovingPieces.add(wildlife);
                game.normal.addPiece(new Position(7, 4), wildlife);
                game.gui.redraw();
                break;

            case WILD_HORSE:
                NPC wildhorse = new WildHorse();

                game.automovingPieces.add(wildhorse);
                game.normal.addPiece(new Position(4, 3), wildhorse);

                game.gui.redraw();
                break;

            case ZOMBIE_APOCALYPSE:
                NPC zombie = new Zombie(true);

                game.automovingPieces.add(zombie);
                game.normal.addPiece(new Position(0, 3), zombie);

                zombie = new Zombie(true);

                game.automovingPieces.add(zombie);
                game.normal.addPiece(new Position(0, 4), zombie);

                zombie = new Zombie(false);

                game.automovingPieces.add(zombie);
                game.normal.addPiece(new Position(7, 3), zombie);

                zombie = new Zombie(false);

                game.automovingPieces.add(zombie);
                game.normal.addPiece(new Position(7, 4), zombie);
                game.gui.redraw();
                break;

            case EVERYONE_UPGRADES:
                for(int i = 0; i < 8; i++){
                    Position pos = new Position(randInt(0, 7), randInt(0, 7));
                    Piece piece = game.normal.getPieceAt(pos);

                    if(piece != null) {
                        Piece.Color color = piece.getColor();

                        if(color == Piece.Color.NPC) continue;

                        game.normal.setPieceAt(pos, switch(piece.getClass().getSimpleName()){
                            case "Pawn" -> new SuicideBomber(color);
                            case "Knight" -> new TrojanHorse(color);
                            case "Rook" -> new RookTower(color);
                            case "Queen" -> new BallQueen(color);
                            case "King" -> new King(color);
                            case "Bishop" -> new Necromancer(color);
                            default -> new Jester(color);
                        });
                    }
                }

                game.gui.redraw();
                break;

            case PITTRAPS:
                for(int i = 0; i < 3; i++){
                    Position pos = new Position(randInt(0, 7), randInt(0, 7));

                    if(game.normal.pieceAt(pos)) continue;

                    game.normal.addPiece(pos, new Pittrap());
                }

                game.gui.redraw();
                break;

            case WHIRLPOOL:
                game.normal.addPiece(new Position(3 + randInt(0, 1), 3 + randInt(0, 1)), new Whirlpool());
                game.gui.redraw();
                break;

            case VOID:
                game.normal.addPiece(new Position(3, 3), new Void(0));
                game.gui.redraw();
                break;

            case METEOR_SHOWER:
                Meteor meteor = new Meteor();

                game.normal.addPiece(new Position(0, 2), meteor);
                game.automovingPieces.add(meteor);

                game.gui.redraw();
                break;
        }
    }

    public static int randInt(int min, int max){
        return Main.random.nextInt(min, max + 1);
    }

    public static char ruleToChar(Rule rule){
        return switch (rule){
            case CENTAUR_MAKING -> 'a';
            case KING_DIES_IN_HELL -> 'b';
            case SUICIDE_BOMBER_HEAVEN -> 'c';
            case NEXT_PIECE_EXPLODES -> 'd';
            case PAWNS_MOVE_FOUR -> 'e';
            case BISHOPS_GAIN_NECROMANCY -> 'f';
            case ZOMBIE_APOCALYPSE -> 'g';
            case GUN -> 'h';
            case WILD_LIFE -> 'i';
            case WILD_HORSE -> 'j';
            case TREADMILL_BOARD -> 'k';
            case MEGA_CASTLE -> 'l';
            case WHIRLPOOL -> 'm';
            case LANDMINES -> 'n';
            case VOID -> 'o';
            case PITTRAPS -> 'p';
            case EVERYONE_UPGRADES -> 'q';
            case GOLD_RUSH -> 'r';
            case METEOR_SHOWER -> 's';
            case UNICORNS -> 't';
            case PORTALS_OPEN -> 'u';
            case PAWN_UPGRADE -> 'v';
            case MORE_GOLD -> 'w';
            case TREASURE -> 'x';
            case POTIONS -> 'y';
        };
    }

    public static Rule charToRule(char rule){
        return switch (rule){
            case 'a' -> Rule.CENTAUR_MAKING;
            case 'b' -> Rule.KING_DIES_IN_HELL;
            case 'c' -> Rule.SUICIDE_BOMBER_HEAVEN;
            case 'd' -> Rule.NEXT_PIECE_EXPLODES;
            case 'e' -> Rule.PAWNS_MOVE_FOUR;
            case 'f' -> Rule.BISHOPS_GAIN_NECROMANCY;
            case 'g' -> Rule.ZOMBIE_APOCALYPSE;
            case 'h' -> Rule.GUN;
            case 'i' -> Rule.WILD_LIFE;
            case 'j' -> Rule.WILD_HORSE;
            case 'k' -> Rule.TREADMILL_BOARD;
            case 'l' -> Rule.MEGA_CASTLE;
            case 'm' -> Rule.WHIRLPOOL;
            case 'n' -> Rule.LANDMINES;
            case 'o' -> Rule.VOID;
            case 'p' -> Rule.PITTRAPS;
            case 'q' -> Rule.EVERYONE_UPGRADES;
            case 'r' -> Rule.GOLD_RUSH;
            case 's' -> Rule.METEOR_SHOWER;
            case 't' -> Rule.UNICORNS;
            case 'u' -> Rule.PORTALS_OPEN;
            case 'v' -> Rule.PAWN_UPGRADE;
            case 'w' -> Rule.MORE_GOLD;
            case 'x' -> Rule.TREASURE;
            default -> Rule.POTIONS;
        };
    }
}
