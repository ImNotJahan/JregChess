package main.java.Networking;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.*;

public class Request {
    HttpURLConnection con;
    public Request(String url, String parameters) {
        try {
            URL urlObj = new URI(url + "?" + parameters).toURL();
            HttpURLConnection con = (HttpURLConnection) urlObj.openConnection();
            con.setRequestMethod("GET");

            this.con = con;
        } catch(Exception e){
            System.out.println(e);
        }
    }

    public void execute() {
        try {
            con.getResponseCode();
        } catch(IOException exception){
            System.out.println(exception);
        }
    }

    public String read(){
        try{
            int responseCode = con.getResponseCode();

            if(responseCode != 200) {
                con.disconnect();
                return null;
            }

            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));

            String inputLine;
            StringBuilder content = new StringBuilder();

            while ((inputLine = in.readLine()) != null)
                content.append(inputLine);

            in.close();

            con.disconnect();

            return content.toString();
        } catch(IOException e){
            System.out.println(e);
            return null;
        }
    }
}
