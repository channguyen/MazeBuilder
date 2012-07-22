import javax.swing.SwingUtilities;

/**
 * Main class
 */
public class Program {
	public static void main(String args[]) {
		// run on event-dispatching thread
		SwingUtilities.invokeLater(new Runnable() {
			@Override
			public void run() {
				MazeBuilder.buildGUI();
			}
		});
	}
}
