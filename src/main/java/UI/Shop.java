package main.java.UI;

import main.java.UI.Components.BuyButton;

import javax.swing.*;
import java.awt.*;

public class Shop extends JFrame {
    private final BuyButton[] products = new BuyButton[11];

    public Shop(){
        setTitle("Shop");
        setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE);
        setLayout(new GridLayout(4, 4));

        init();

        pack();
        setVisible(true);
        setResizable(false);
    }

    private void init(){
        products[0] = new BuyButton("pawn", 2);
        products[1] = new BuyButton("rook", 7);
        products[2] = new BuyButton("knight", 5);
        products[3] = new BuyButton("bishop", 6);
        products[4] = new BuyButton("king", 9);
        products[5] = new BuyButton("queen", 12);
        products[6] = new BuyButton("zebra", 7);
        products[7] = new BuyButton("rook-knight", 9);
        products[8] = new BuyButton("bishop-knight", 6);
        products[9] = new BuyButton("knight-queen", 15);
        products[10] = new BuyButton("giraffe", 6);

        for(BuyButton product : products) add(product);
    }


}
