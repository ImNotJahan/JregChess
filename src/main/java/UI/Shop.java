package main.java.UI;

import javax.swing.*;
import java.awt.*;

public class Shop extends JFrame {
    public Shop(){
        setTitle("Shop");
        setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE);
        setLayout(new FlowLayout());

        init();

        pack();
        setVisible(true);
    }

    private void init(){
        add(new Label("Buy things"));
    }
}
