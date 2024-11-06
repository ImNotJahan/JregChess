package main.java.UI;

import javax.swing.*;

public class Healthbar extends JFrame {
    JProgressBar bar = new JProgressBar();

    public Healthbar(String title, int maxHealth) {
        setTitle("Healthbar");
        setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
        setLayout(new BoxLayout(getContentPane(), BoxLayout.Y_AXIS));

        bar.setMaximum(maxHealth);
        bar.setValue(maxHealth);

        add(new JLabel(title));
        add(bar);

        pack();
        setVisible(true);
        setResizable(false);
    }

    public void set(int to) {
        bar.setValue(to);

        if(to == 0) {
            setVisible(false);
            dispose();
        }
    }
}
