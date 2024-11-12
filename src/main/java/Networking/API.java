package main.java.Networking;

public class API {
    private static final String base = "https://jahanrashidi.com/chess/";

    public static void newGame(String seed){
        new Request(base + "new_game.php", "code=" + seed).execute();
    }

    /**
     *
     * @param seed The randomness/identifying seed of the game
     * @return True if white's turn, false if black's or failure
     */
    public static boolean getTurn(String seed){
        Request request = new Request(base + "get_turn.php", "code=" + seed);
        String result = request.read();

        if(result == null) return false;
        return !result.equals("0"); // 0 is false, anything else true
    }

    public static String getMove(String seed){
        return new Request(base + "get_move.php", "code=" + seed).read();
    }

    public static void move(String seed, String move){
        new Request(base + "move.php", "code=" + seed + "&move=" + move).execute();
    }
}
