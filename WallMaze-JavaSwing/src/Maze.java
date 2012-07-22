import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.util.ArrayList;
import java.util.List;

public class Maze {
	/*
	 * Maze row
	 */
	public static final int ROW = 15;

	/*
	 * Maze Column
	 */
	public static final int COLUMN = 15;

	/*
	 * Number of vertices
	 */
	public static int VERTICES = ROW * COLUMN;
	
	/*
	 * Length of edge
	 */
	public static int SPACING = 40;
	
	/*
	 * Spacing from the top left corner (0, 0)
	 */
	public static int SHIFTING = 50;
	
	/*
	 * The goal
	 */
	public static int GOAL_VERTEX = VERTICES - 1;

	/*
	 * The marked drawing map
	 */
	public boolean[][] mMap = new boolean[VERTICES][VERTICES];

	/*
	 * Vertex information if mExplored[u] == true vertex u is already visited
	 */
	public boolean[] mExplored = new boolean[VERTICES];

	/*
	 * Vertex information for solver if mExplored[u] == true vertex u is already
	 * visited
	 */
	public boolean[] mTurtleExplored = new boolean[VERTICES];
	
	/*
	 * Color of removed edge
	 */
	private Color mRemovedColor;
	
	public Maze() {
		mRemovedColor = Color.decode("#EEEEEE");
		reset();
	}
	
	public void reset() {
		initExplored();
		initGraph();
	}
	
	/**
	 * Initialize vertices
	 * 		false: unexplored
	 * 		true: explored
	 */
	private void initExplored() {
		for (int i = 0; i < VERTICES; ++i) { 
			mExplored[i] = false;
			mTurtleExplored[i] = false;
		}
	}
	
	/**
	 * Initialize all marked to true:
	 * 		if mGraph[x][y] = true then 
	 * 			draw a line from x -> y
	 *
	 * 		neighbor coordinates
	 * 		-------------------------------------
	 * 		|	 		|  			|			|
	 * 		|			| (x-1, y)	|			|			
	 * 		|			|			|			|
	 * 		-------------------------------------
	 * 		|	 		|  			|			|
	 * 		| (x, y-1)  | (x, y)	| (x, y+1)	|			
	 * 		|			|			|			|
	 * 		-------------------------------------
	 * 		|	 		|  			|			|
	 * 		|			| (x+1, y)	|			|			
	 * 		|			|			|			|
	 * 		-------------------------------------
	 *
	 */
	private void initGraph() {
		// initialize all marked to false
		int vertices = ROW * COLUMN;
		for (int u = 0; u < vertices; ++u) {
			for (int v = 0; v < vertices; ++v) {
				mMap[u][v] = false;
			}
		}
	}
	
	/**
	 * Check if a square is in a maze
	 *
	 * @param x
	 * 			the x coordinate in JPanel
	 * @param y
	 *			the y coordinate in JPanel
	 *
	 * @return	
	 * 			true if it's in maze
	 * 			false otherwise
	 */
	public boolean isInMaze(int x, int y) {
		return (x >= 0 && x < ROW && y >= 0 && y < COLUMN);
	}
	
	/**
	 * Get the number of neighbors of a vertex 
	 * 
	 * @param u
	 * 		vertex
	 * 
	 * @return neighbors
	 * 		an array list of int 	
	 */
	public List<Integer> getTurtleNeighbors(int u) {
		List<Integer> neighbors = new ArrayList<Integer>();
		Point p = toPoint(u);
		int to = -1;

		// up
		if (isInMaze(p.mX - 1, p.mY)) {
			to = toVertex(p.mX - 1, p.mY);
			if (!mTurtleExplored[to])
				neighbors.add(to);
		}

		// down 
		if (isInMaze(p.mX + 1, p.mY)) {
			to = toVertex(p.mX + 1, p.mY);
			if (!mTurtleExplored[to])
				neighbors.add(to);
		}

		// left 
		if (isInMaze(p.mX,  p.mY - 1)) {
			to = toVertex(p.mX, p.mY - 1);
			if (!mTurtleExplored[to])
				neighbors.add(to);
		}

		// right 
		if (isInMaze(p.mX, p.mY + 1)) {
			to = toVertex(p.mX, p.mY + 1);
			if (!mTurtleExplored[to])
				neighbors.add(to);
		}

		return neighbors;
	}
	
	/**
	 * Convert a coordinate (x, y) to a vertex on graph
	 *
	 *	 		-------------
	 * 			| 0	| 1	| 2	|
	 *          -------------
	 *          | 3	| 4	| 5	|
	 *          -------------
	 *          | 6	| 7	| 8	|
	 *          -------------
	 *
	 *          1) formula: 
	 *          	--------------------------
	 *          	-  vertex = x * ROW + y  -
	 *				--------------------------
	 *
	 *          2) check:
	 *          ROW = 3, COLUMN = 3
	 *			then	
	 *			[0][0] = 0 * 3 + 0 = 0
	 *			[0][1] = 0 * 3 + 1 = 1
	 *			[0][2] = 0 * 3 + 2 = 2
	 *
	 *			[1][0] = 1 * 3 + 0 = 3
	 *			[1][1] = 1 * 3 + 1 = 4
	 *			[1][2] = 1 * 3 + 2 = 5
	 *
	 *			[2][0] = 2 * 3 + 0 = 6
	 *			[2][1] = 2 * 3 + 1 = 7
	 *			[2][2] = 2 * 3 + 2 = 8
	 *
	 * @param x
	 * 			x coordinate
	 * @param y 
	 * 			y coordinate
	 * 
	 * @return 
	 * 		a vertex
	 *
	 */
	public int toVertex(int x, int y) {
		return (x * ROW + y);
	}
	
	/**
	 * Convert from a vertex v to a pair (x, y)
	 *
	 * 			1) formula: 
	 * 				-----------------
	 * 				-  x = v / ROW  -
	 * 				-  y = v % ROW  -
	 *			    -----------------
	 *
	 *			2) check:
	 *				0 and [0][0]
	 *					x = 0 / 3 = 0
	 *					y = 0 % 3 = 0
	 *
	 *				1 and [0][1]
	 *					x = 0 / 3 = 0
	 *					y = 1 % 3 = 1
	 *
	 * 				6 and [2][0]
	 *					x = 6 / 3 = 2
	 *					y = 0 % 3 = 0
	 *
	 *				8 and [0][1]
	 *					x = 8 / 3 = 2
	 *					y = 8 % 3 = 2
	 *
	 *	@param v
	 *			vertex
	 *
	 *	@return
	 *			a point 
	 */
	public Point toPoint(int v) {
		return new Point(v / ROW, v % COLUMN);
	}
	
	/**
	 * Get the number of neighbors of a vertex 
	 * 
	 * @param u
	 * 		vertex
	 * 
	 * @return neighbors
	 * 		vertex neighbor of u
	 */
	public List<Integer> getNeighbors(int u) {
		List<Integer> neighbors = new ArrayList<Integer>();
		Point p = toPoint(u);
		int to = -1;

		// up
		if (isInMaze(p.mX - 1, p.mY)) {
			to = toVertex(p.mX - 1, p.mY);
			if (!mExplored[to])
				neighbors.add(to);
		}

		// down 
		if (isInMaze(p.mX + 1, p.mY)) {
			to = toVertex(p.mX + 1, p.mY);
			if (!mExplored[to])
				neighbors.add(to);
		}

		// left 
		if (isInMaze(p.mX,  p.mY - 1)) {
			to = toVertex(p.mX, p.mY - 1);
			if (!mExplored[to])
				neighbors.add(to);
		}

		// right 
		if (isInMaze(p.mX, p.mY + 1)) {
			to = toVertex(p.mX, p.mY + 1);
			if (!mExplored[to])
				neighbors.add(to);
		}

		return neighbors;
	}
	
	/**
	 * Check if there still unvisited vertices
	 * 
	 * @return true/false
	 */
	public boolean hasnoMoreVertices() {
		for (int i = 0; i < VERTICES; ++i) {
			if (mTurtleExplored[i] == false)
				return false;
		}
		return true;
	}
	
	
	/**
	 * Draw the whole maze
	 * 
	 * @param g
	 * 			graphics handler
	 */
	public void draw(Graphics g) {
		Graphics2D g2D = (Graphics2D) g;      
		g2D.setStroke(new BasicStroke(3f));
		g.setColor(Color.black);
		for (int x = 0; x < ROW; ++x) {
			for (int y = 0; y < COLUMN; ++y) {
				Point p = new Point(x, y);
				drawNode(g, p);
			}
		}
	
		// for each vertex u, we check its 4 neighbors
		int left;
		int right;
		int up;
		int down;
		
		Point from;
		Point to;
		
		for (int u = 0; u < VERTICES; ++u) {
			int c = u % COLUMN;
			int r = u / ROW;
			
			left  = -1;
			right = -1;
			up    = -1;
			down  = -1;
			
			from = toPoint(u);
			
			// left
			if (c - 1 >= 0) {
				left = r * ROW + c - 1;
			}
		
			// right
			if (c + 1 <= COLUMN - 1) {
				right = r * ROW + c + 1;
			}
		
			// down
			if (r - 1 >= 0) {
				down = (r - 1) * ROW + c;
			}
			
			// up
			if (r + 1 <= ROW - 1) {
				up = (r + 1) * ROW + c;
			}
		
    		if (left != -1) {
    			to = toPoint(left);
    			if (mMap[u][left]) 
    				drawRemoveEdge(g, from, to);
    		}
    		
    		if (right != -1) {
    			to = toPoint(right);
    			if (mMap[u][right]) 
    				drawRemoveEdge(g, from, to);
    		}
    		
    		if (up != -1) {
    			to = toPoint(up);
    			if (mMap[u][up]) 
    				drawRemoveEdge(g, from, to);
    		}
    		
    		if (down != -1) {
    			to = toPoint(down);
    			if (mMap[u][down]) 
    				drawRemoveEdge(g, from, to);
    		}
		}
		
		for (int i = 0; i < VERTICES; ++i) {
			if (mExplored[i]) {
				Point p = toPoint(i);
				g.setColor(Color.red);
				g.drawOval(p.mX * SPACING + 17 + SHIFTING, p.mY * SPACING +  17 + SHIFTING, 2, 2);
			}
		}
		
		
		Point end = toPoint(VERTICES - 1);
		g.setColor(Color.yellow);
		g.drawOval(end.mX * SPACING + 17 + SHIFTING, end.mY * SPACING +  17 + SHIFTING, 2, 2);
		
		// this is the only way out
		g.setColor(mRemovedColor);
		Point p = new Point((ROW - 1) * SPACING + SHIFTING, (COLUMN - 1) * SPACING + SHIFTING); 
		g.drawLine(p.mX + SPACING, p.mY, p.mX + SPACING, p.mY + SPACING);
	}
	
	
	/**
	 * Remove an edge from to point
	 * 
	 * @param g
	 * 			graphics component
	 * 
	 * @param cur
	 * 			the current vertex
	 * 
	 * @param adj
	 * 			its neighbor
	 */
	private void drawRemoveEdge(Graphics g, Point cur, Point adj) {
		g.setColor(mRemovedColor);
		Point p = new Point(cur.mX * SPACING + SHIFTING, cur.mY * SPACING + SHIFTING); 
		// right
		if (cur.mX + 1 == adj.mX && cur.mY == adj.mY) {
			g.drawLine(p.mX + SPACING, p.mY, p.mX + SPACING, p.mY + SPACING);
		}
		// left
		else if (cur.mX - 1 == adj.mX && cur.mY == adj.mY) {
			g.drawLine(p.mX, p.mY, p.mX, p.mY + SPACING);
		}
		// top
		else if (cur.mX == adj.mX && cur.mY - 1 == adj.mY) {
			g.drawLine(p.mX, p.mY, p.mX + SPACING, p.mY);
		}
		else if (cur.mX == adj.mX && cur.mY + 1 == adj.mY){
			g.drawLine(p.mX, p.mY + SPACING, p.mX + SPACING, p.mY + SPACING);
		}
	}
	
	/**
	 * Draw a box around one point
	 * 
	 * @param g
	 * 			graphics component
	 * 
	 * @param p
	 * 			the point to be drawn
	 */
	private void drawNode(Graphics g, Point p) {
		// add spacing
		p.mX = p.mX * SPACING + SHIFTING;
		p.mY = p.mY * SPACING + SHIFTING;
		
		// draw top
		g.drawLine(p.mX, p.mY, p.mX + SPACING, p.mY);
		// draw left
		g.drawLine(p.mX, p.mY, p.mX, p.mY + SPACING);
		// draw right
		g.drawLine(p.mX + SPACING, p.mY, p.mX + SPACING, p.mY + SPACING);
		// draw bottom
		g.drawLine(p.mX, p.mY + SPACING, p.mX + SPACING, p.mY + SPACING);
	}
	
	/**
	 * Draw a box around one point
	 * 
	 * @param g
	 * 			graphics component
	 * 
	 * @param p
	 * 			the point to be drawn
	 */
	public void drawVisitedNode(Graphics g, Point p) {
		g.setColor(Color.green);
		g.drawOval(p.mX * SPACING + 17 + SHIFTING, p.mY * SPACING + 17 + SHIFTING, 2, 2);
	}
}
