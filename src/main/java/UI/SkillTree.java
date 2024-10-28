package main.java.UI;

import main.java.Pieces.*;
import main.java.Pieces.NPCs.Placeholder;
import main.java.Pieces.Upgrades.BishopKnight;
import main.java.Pieces.Upgrades.KnightQueen;
import main.java.Pieces.Upgrades.RookKnight;
import main.java.Pieces.Upgrades.SuicideBomber;

import javax.swing.*;
import java.awt.*;

public class SkillTree extends JFrame {
    private final UpgradeButton[] upgrades = new UpgradeButton[13];

    public SkillTree(){
        setTitle("Skill Tree");
        setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE);
        setLayout(new GridLayout(4, 4));

        init();

        pack();
        setVisible(true);
    }

    private void init(){
        upgrades[0] = new UpgradeButton("suicide-bomber", Pawn.class.getSimpleName());
        upgrades[1] = new UpgradeButton("centaur", Pawn.class.getSimpleName());
        upgrades[2] = new UpgradeButton("unicorn", Knight.class.getSimpleName());
        upgrades[3] = new UpgradeButton("trojan-horse", Knight.class.getSimpleName());
        upgrades[4] = new UpgradeButton("rook-knight", Knight.class.getSimpleName(), Rook.class.getSimpleName());
        upgrades[5] = new UpgradeButton("bishop-knight", Bishop.class.getSimpleName(), Knight.class.getSimpleName());
        upgrades[6] = new UpgradeButton("necromancer", Pawn.class.getSimpleName());
        upgrades[7] = new UpgradeButton("super-bishop", Pawn.class.getSimpleName());
        upgrades[8] = new UpgradeButton("super-king", Pawn.class.getSimpleName());
        upgrades[9] = new UpgradeButton("ball-queen", Pawn.class.getSimpleName());
        upgrades[10] = new UpgradeButton("knight-queen", Queen.class.getSimpleName(), Knight.class.getSimpleName());
        upgrades[11] = new UpgradeButton("angry-rook", Pawn.class.getSimpleName());
        upgrades[12] = new UpgradeButton("rook-tower", Pawn.class.getSimpleName());

        for(UpgradeButton upgrade : upgrades) add(upgrade);

        add(new JLabel("Each"));
        add(new JLabel("Costs"));
        add(new JLabel("5GP"));
    }

    public static Piece pieceFromId(String id){
        return switch (id){
            case "suicide-bomber" -> new SuicideBomber(Piece.Color.White);
            case "knight-queen" -> new KnightQueen(Piece.Color.White);
            case "bishop-knight" -> new BishopKnight(Piece.Color.White);
            case "rook-knight" -> new RookKnight(Piece.Color.White);
            default -> new Placeholder();
        };
    }
}
