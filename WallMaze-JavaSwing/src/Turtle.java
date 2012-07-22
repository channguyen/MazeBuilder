import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.geom.AffineTransform;


/**
 * An object that can be drawn in maze
 */
public class Turtle {
	private int mXPos;
	private int mYPos;
	private Color mColor;
	
	public Turtle(Color c) {
		mXPos = 0;
		mYPos = 0;
		mColor = c;
	}
	
	public void setPosition(int x, int y) {
		mXPos = x;
		mYPos = y;
	}
	
	public int getXPos() {
		return mXPos;
	}
	
	public int getYPos() {
		return mYPos;
	}
	
	public void draw(Graphics g) {
		int width = 15;
		int height = 18;
		int heading = 0;
		int xPos = mXPos;
		int yPos = mYPos;
		
		// cast to 2d object
	    Graphics2D g2 = (Graphics2D) g;
		// save the current transformation
	    AffineTransform oldTransform = g2.getTransform();
	      
	    // rotate the turtle and translate to xPos and yPos
	    g2.rotate(Math.toRadians(heading), xPos, yPos);
	      
	    // determine the half width and height of the shell
	    int halfWidth = (int) (width/2); 		// of shell
	    int halfHeight = (int) (height/2); 		// of shell
	    int quarterWidth = (int) (width/4); 	// of shell
	    int thirdHeight = (int) (height/3); 	// of shell
	    int thirdWidth = (int) (width/3); 		// of shell
	      
	      // draw the body parts (head)
	    g2.setColor(mColor);
	    g2.fillOval(xPos - quarterWidth, yPos - halfHeight - (int) (height/3), halfWidth, thirdHeight);
	    g2.fillOval(xPos - (2 * thirdWidth), yPos - thirdHeight, thirdWidth, thirdHeight);
	    g2.fillOval(xPos - (int) (1.6 * thirdWidth), yPos + thirdHeight, thirdWidth, thirdHeight);
	    g2.fillOval(xPos + (int) (1.3 * thirdWidth), yPos - thirdHeight, thirdWidth, thirdHeight);
	    g2.fillOval(xPos + (int) (0.9 * thirdWidth), yPos + thirdHeight, thirdWidth, thirdHeight);
	                  
	    // draw the shell
	    g2.setColor(mColor);
	    g2.fillOval(xPos - halfWidth, yPos - halfHeight, width, height);
	    // reset the transformation matrix
	    g2.setTransform(oldTransform);
	}
	
}
