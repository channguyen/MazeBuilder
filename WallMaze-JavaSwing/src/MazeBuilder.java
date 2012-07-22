import java.awt.Color;
import java.awt.Graphics;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Random;
import java.util.List;

import javax.swing.JFrame;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JOptionPane;
import javax.swing.JPanel;


public class MazeBuilder extends JPanel {
	/*
	 * State of a process
	 */
	private enum MazeState {
		NotStart,
		Running,
		Complete
	}
	
	/*
	 * The waiting time for repaint
	 */
	private static int BUILD_INTERVAL = 5;
	
	/*
	 * Delay time
	 */
	private static int SOLVE_INTERVAL = 5;
	
	/*
	 * Parent frame
	 */
	private JFrame mFrame;
	
	/*
	 * Random generator
	 */
	private Random mRandom;
	
	/*
	 * Current position of turtle
	 */
	private int mCurrentVertex = 0;
	
	/*
	 * The turtle for maze solver
	 */
	private Turtle mSolver;
	
	/*
	 * Maze builder 
	 */
	private Maze mMaze;
	
	/*
	 * State of building maze process
	 */
	private MazeState mBuildState;
	
	/*
	 * State of solving maze process
	 */
	private MazeState mSolveState;
	
	
	/*
	 * Actual path
	 */
	private int[] mPath;
	
	/**
	 * Constructor
	 * 
	 * @param frame
	 * 			the parent frame
	 */
	public MazeBuilder(JFrame frame) {
		mFrame = frame;		
		// initialize random
		mRandom = new Random();
		mSolver = new Turtle(Color.red);
		mMaze = new Maze();
		
		mPath = new int[Maze.VERTICES];
		
		mBuildState = MazeState.NotStart;
		mSolveState = MazeState.NotStart;
		
		buildMenu(mFrame);
	}
	
	/**
	 * Reset all data of maze and repaint
	 */
	public void reset() {
		mMaze.reset();
		mBuildState = MazeState.NotStart;
		mSolveState = MazeState.NotStart;
		repaint();
	}
	
	/**
	 * Delay the current thread for a specific
	 * amount of time
	 * 
	 * @param delayTime
	 * 				time in millisecond
	 */
	private void delay(int delayTime) {
		try {
			Thread.sleep(delayTime);
		} 
		catch (InterruptedException e) {
			System.err.println("Error in delay");
		}
	}
	
	/**
	 * Backtracking to solve maze from a given 
	 * vertex u
	 * 
	 * @param u
	 * 			starting point
	 */
	private void solveMazeBacktracking(int u) {
		mCurrentVertex = u;
		
		if (u == Maze.GOAL_VERTEX) {
			mSolveState = MazeState.Complete;
			repaint();
			return;
		}
	
		if (mMaze.hasnoMoreVertices()) {
			return;
		}
		
		mMaze.mTurtleExplored[u] = true;
		
		// for each neighbor of u make a move
		List<Integer> neighbors = mMaze.getTurtleNeighbors(u);
		for (int i = 0; i < neighbors.size(); ++i) {
			int v = neighbors.get(i);
			if (mMaze.mMap[u][v] == true || mMaze.mMap[v][u] == true) {
				if (mSolveState != MazeState.Complete) {
					// store actual path
					mPath[v] = u;
					
					delay(SOLVE_INTERVAL);
					repaint();
					solveMazeBacktracking(v);
				}
			}
		}
	}
	

	/**
	 * DFS algorithm to remove edges
	 *
	 * @param u
	 * 			starting vertex
	 *  
	 */
	private void buildMazeDfs(int u) {
		// mark that vertex as visited
		mMaze.mExplored[u] = true;
		
		// get all neighbors
		List<Integer> neighbors = mMaze.getNeighbors(u);
		
		int v = -1;
		// get a random vertex v
		if (neighbors.size() > 0) {
			int idx = mRandom.nextInt(neighbors.size());
			v = neighbors.get(idx);
		}
	
		if (v == -1) {
			mBuildState = MazeState.Complete;
			repaint();
			return; 
		}
		
		// remove edge from u to v
		mMaze.mMap[u][v] = true;
		mMaze.mMap[v][u] = true;
		
		// loop through all u's neighbors 
		for (int n = 0; n < neighbors.size(); ++n) {
			// get the next neighbor
			int next = neighbors.get(n);
			// if it's not explored
			if (mMaze.mExplored[next] == false) {
				delay(BUILD_INTERVAL);
				repaint();
				buildMazeDfs(next);
			}
		}		
	}
	
	/**
	 * Build the menu bar and add
	 * it to frame f
	 * 
	 * @param f
	 * 			frame to hold menu bar
	 */
	public void buildMenu(JFrame f) {
		JMenuBar bar = new JMenuBar();
		JMenu optionMenu = new JMenu("Option");
		JMenuItem item;
		
		item = new JMenuItem("Build");
		item.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				(new Thread() {
					public void run() {
						mBuildState = MazeState.Running;
						buildMazeDfs(0);
					}
				}).start();
			}
		});
		optionMenu.add(item);
		
		item = new JMenuItem("Solve");
		item.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (mBuildState == MazeState.Complete) {
					(new Thread() {
						public void run() {
							mSolveState = MazeState.Running;
							solveMazeBacktracking(0);
						}
					}).start();
				}
				else {
					JOptionPane.showMessageDialog(mFrame, "You need to build your maze first!");
				}
			}
		});
		optionMenu.add(item);
		
		item = new JMenuItem("Reset");
		item.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				// only reset if maze is complete built or solved
				if (mBuildState != MazeState.Running && mSolveState != MazeState.Running) {
					reset();
				}
				else {
					if (mBuildState == MazeState.Running) {
						JOptionPane.showMessageDialog(mFrame, "Building maze is in process!");
					}
					else if (mSolveState == MazeState.Running) {
						JOptionPane.showMessageDialog(mFrame, "Solving maze is in process!");
					}
				}
			}
		});
		optionMenu.add(item);
		
		bar.add(optionMenu);
		f.setJMenuBar(bar);
	}
	
	@Override
	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		mMaze.draw(g);
		
		// if the maze is built, draw a turtle 
		// and all visited nodes
		if (mBuildState == MazeState.Complete) {
			Point p = mMaze.toPoint(mCurrentVertex);
			mSolver.setPosition(p.mX * Maze.SPACING + 17 + Maze.SHIFTING, p.mY * Maze.SPACING + 17 + Maze.SHIFTING);
			mSolver.draw(g);
			
			for (int i = 0; i < Maze.VERTICES; ++i) {
				if (mMaze.mTurtleExplored[i]) {
					mMaze.drawVisitedNode(g, mMaze.toPoint(i));
				}
			}
		}
		
		if (mSolveState == MazeState.Complete) {
			int u = Maze.GOAL_VERTEX;
			while (mPath[u] != 0) {
				Point o = mMaze.toPoint(mPath[u]);
				g.setColor(Color.blue);
				g.drawRect(o.mX * Maze.SPACING + 17 + Maze.SHIFTING, o.mY * Maze.SPACING + 17 + Maze.SHIFTING, 4, 4);
				u = mPath[u];
			}
		}
	}
	
	/**
	 * Build entire UI
	 */
	public static void buildGUI() {
		// create a container level JFrame
		JFrame frame = new JFrame("Maze Generator");
	
		// set up frame
		frame.setSize(800, 800);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setVisible(true);
		
		// create a panel
		MazeBuilder app = new MazeBuilder(frame);
		app.buildMenu(frame);
		frame.setContentPane(app);
		
	}
}
