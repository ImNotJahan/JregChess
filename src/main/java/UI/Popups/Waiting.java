package main.java.UI.Popups;

import javax.swing.*;
import java.awt.*;

public class Waiting extends JDialog {
    public Waiting(){
        setTitle("Waiting");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new FlowLayout());

        add(new JLabel("Waiting for other player to move..."));

        pack();
        setVisible(true);
        setResizable(false);
    }

    public void close(){
        setVisible(false);
        dispose();
    }
}
