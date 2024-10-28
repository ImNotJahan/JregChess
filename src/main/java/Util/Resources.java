package main.java.Util;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.net.URL;

public class Resources {
    public BufferedImage loadBufferedImage(String filename) throws IOException {
        URL url = getClass().getResource("/main/resources/" + filename);
        return ImageIO.read(url);
    }
}
