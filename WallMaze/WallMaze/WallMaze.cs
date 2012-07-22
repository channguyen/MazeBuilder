using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace WallMaze {
    class WallMaze : Form {
        private enum MazeState {
            NOT_START,
            BUILDING,
            BUILT,
            SOLVING,
            SOLVED
        };

        private Maze maze;
        private Square currentSquare;
        private Square.Direction lastSide;
        private Random random;
        private bool addingSquareFlag;
        private List<Square.Direction> directions;

        /**
         * GUI components
         */
        private MazeInfoDialog sizeDialog;
        private MenuItem runItem;
        private MenuItem solveItem;
        private MenuItem resetItem;
        private MenuItem setSizeItem;
        private MenuItem startItem;
        private MenuItem endItem;
        private ContextMenu popupMenu;
        private MouseEventArgs popUpMouseEvent;

        private MazeState mazeState;

        /**
         * Solve maze data
         */
        private int startNode;
        private int endNode;
        private int currentNode;
        private bool[,] adjacentMatrix;
        private bool[] visited;
        private int[] path;
        private int nodes;

        private static Pen pen;
        private static Font font;
        private static SolidBrush brush;

        /// <summary>
        /// Constructor
        /// </summary>
        public WallMaze() {
            pen = new Pen(Brushes.Black);
            pen.Width = 3.0f;

            font = new Font("Arial", 8);
            brush = new SolidBrush(Color.Red);

            Width = 800;
            Height = 800;
            random = new Random();

            startNode = -1;
            endNode = -1;
            currentNode = -1;

            mazeState = MazeState.NOT_START;

            addingSquareFlag = true;

            directions = new List<Square.Direction>();
            sizeDialog = new MazeInfoDialog();

            DoubleBuffered = true;

            SetupPopupMenu();
            SetupMenu();
        }

        private void SetupPopupMenu() {
            popupMenu = new ContextMenu();
            startItem = new MenuItem();
            startItem.Text = "Start";
            popupMenu.MenuItems.Add(startItem);
            startItem.Click += delegate(object sender, EventArgs e) {
                int x = popUpMouseEvent.X / maze.Spacing;
                int y = popUpMouseEvent.Y / maze.Spacing;
                startNode = ConvertToNode(y, x);

                // enable solve button
                if (startNode != -1 && endNode != -1) {
                    solveItem.Enabled = true;
                }

                Invalidate();
            };

            endItem = new MenuItem();
            endItem.Text = "End";
            popupMenu.MenuItems.Add(endItem);
            endItem.Click += delegate(object sender, EventArgs e) {
                int x = popUpMouseEvent.X / maze.Spacing;
                int y = popUpMouseEvent.Y / maze.Spacing;
                endNode = ConvertToNode(y, x);

                // enable solve button
                if (startNode != -1 && endNode != -1) {
                    solveItem.Enabled = true;
                }

                Invalidate();
            };

            this.MouseDown += delegate(object sender, MouseEventArgs e) {
                if (mazeState == MazeState.BUILT) {
                    popupMenu.Show(this, new Point(e.X, e.Y));
                    popUpMouseEvent = e;
                }
            };
        }

        private void SetupMenu() {
            MainMenu menu = new MainMenu();
            this.Menu = menu;

            MenuItem buildMenu = new MenuItem();
            menu.MenuItems.Add(buildMenu);
            buildMenu.Text = "Build";

            runItem = new MenuItem();
            runItem.Enabled = false;
            buildMenu.MenuItems.Add(runItem);
            runItem.Text = "Run";
            runItem.Click += delegate(object sender, EventArgs args) {
                Thread thread = new Thread(new ThreadStart(BuildMaze));
                mazeState = MazeState.BUILDING;
                thread.Start();
            };

            solveItem = new MenuItem();
            solveItem.Enabled = false;
            buildMenu.MenuItems.Add(solveItem);
            solveItem.Text = "Solve";
            solveItem.Click += delegate(object sender, EventArgs args) {
                Thread thread = new Thread(new ThreadStart(SolveMaze));
                mazeState = MazeState.SOLVING;
                thread.Start();
            };

            resetItem = new MenuItem();
            resetItem.Enabled = false;
            buildMenu.MenuItems.Add(resetItem);
            resetItem.Text = "Reset";
            resetItem.Click += delegate(object sender, EventArgs args) {
                mazeState = MazeState.NOT_START;
                runItem.Enabled = false;
                solveItem.Enabled = false;
                /*
                 * Reset data
                 */
                currentNode = -1;
                startNode = -1;
                endNode = -1;

                Invalidate();
            };

            setSizeItem = new MenuItem();
            buildMenu.MenuItems.Add(setSizeItem);
            setSizeItem.Text = "Set Size";
            setSizeItem.Click += delegate(object sender, EventArgs args) {
                sizeDialog.ShowDialog();
                if (sizeDialog.DialogResult == DialogResult.OK) {
                    maze = new Maze(sizeDialog.Width, sizeDialog.Height, sizeDialog.Spacing);
                    runItem.Enabled = true;
                    resetItem.Enabled = true;

                    mazeState = MazeState.BUILT;

                    InitializeGraph();

                    Invalidate();
                }
            };
        }

        public void BuildMaze() {
            // pick a random square
            int c = 0;
            int r = random.Next(maze.Height);
            Square startingSquare = maze.SquareAt(r, c);

            // set current square to starting square
            currentSquare = startingSquare;

            // remove LEFT wall of current square
            currentSquare.RemoveSide(Square.Direction.LEFT);

            // set last side to left
            lastSide = Square.Direction.LEFT;

            // while there remains any blocked square in the grid
            while (maze.IsAnyBlocked()) {
                // set bool addingSquare to false 
                addingSquareFlag = false;

                // create a random sequence of the 0, 1, 2, or 3 remaining walls for currentSquare
                // exclude sides that lead to unblocked squares (squares that are already in the maze)
                directions.Clear();
                if (currentSquare.HasSide(Square.Direction.LEFT)) {
                    if (currentSquare.AdjacentSquare(Square.Direction.LEFT) != null &&
                        currentSquare.AdjacentSquare(Square.Direction.LEFT).IsBlocked()) {
                        directions.Add(Square.Direction.LEFT);
                    }
                }

                if (currentSquare.HasSide(Square.Direction.RIGHT)) {
                    if (currentSquare.AdjacentSquare(Square.Direction.RIGHT) != null &&
                        currentSquare.AdjacentSquare(Square.Direction.RIGHT).IsBlocked()) {
                        directions.Add(Square.Direction.RIGHT);
                    }
                }

                if (currentSquare.HasSide(Square.Direction.TOP)) {
                    if (currentSquare.AdjacentSquare(Square.Direction.TOP) != null &&
                        currentSquare.AdjacentSquare(Square.Direction.TOP).IsBlocked()) {
                        directions.Add(Square.Direction.TOP);
                    }
                }

                if (currentSquare.HasSide(Square.Direction.BOTTOM)) {
                    if (currentSquare.AdjacentSquare(Square.Direction.BOTTOM) != null &&
                        currentSquare.AdjacentSquare(Square.Direction.BOTTOM).IsBlocked()) {
                        directions.Add(Square.Direction.BOTTOM);
                    }
                }

                // exclude lastSide
                directions.Remove(lastSide);

                // shuffle them
                Shuffle(directions);

                foreach (Square.Direction dir in directions) {
                    // let nextSide be the current element in this sequence
                    Square.Direction nextSide = dir;
 
                    // if square adjacent to the current square on side nextSide exists and is blocked
                    if (currentSquare.AdjacentSquare(nextSide) != null && currentSquare.AdjacentSquare(nextSide).IsBlocked()) {
                        // remove walls that separate currentSquare and adjacentSquare
                        Square adjacent = currentSquare.AdjacentSquare(nextSide);
                        currentSquare.RemoveSide(nextSide);
                        adjacent.RemoveSide(Square.OppositeSide(nextSide));

                        // add path to adjacency matrix
                        int u = ConvertToNode(currentSquare.Row, currentSquare.Column);
                        int v = ConvertToNode(adjacent.Row, adjacent.Column);
                        adjacentMatrix[u, v] = true;
                        adjacentMatrix[v, u] = true;

                        // adjacentSquare becomes currentSquare
                        currentSquare = adjacent;


                        // set boolean addingSquareFlag to true
                        addingSquareFlag = true;

                        // set lastSide to the opposite of nextSide
                        lastSide = Square.OppositeSide(nextSide);
                        break;
                    }
                }

                // this means we couldn't find any adjacent square to move to in the for loop above
                // we now have to backtrack by picking a random unblocked square elsewhere in the maze
                // and try to build a path from that point
                if (!addingSquareFlag) {
                    // pick an unblocked square (already in the maze) from the grid at
                    // random, optionally ensuring that it is adjacent to at least one
                    // blocked square (not already in the maze)
                    // this becomes the current square
                    currentSquare = maze.SelectRandomUnblockedSquare();
                    if (currentSquare == null)
                        break;

                    // randomly pick an open side (one with wall already removed) of the 
                    // current square, this becomes lastSide
                    lastSide = currentSquare.SelectOpenSide(); 
                }

                // redraw
                UpdateDrawing();
            }

            // update state
            mazeState = MazeState.BUILT;
        }

        private void UpdateDrawing() {
            Thread.Sleep(10);
            Invalidate();
        }

        /// <summary>
        /// Sloppy shuffle algorithm
        /// </summary>
        /// <param name="directions"></param>
        private void Shuffle(List<Square.Direction> directions) {
            for (int i = 0; i < directions.Count; ++i) {
                // get a random idx
                int idx = random.Next(directions.Count);

                // swap them
                Square.Direction temp = directions[i];
                directions[i] = directions[idx];
                directions[idx] = temp;
            }
        }

        private void SolveMaze() {
            DepthFirstSearch(startNode);
        }

        private void DepthFirstSearch(int u) {
            currentNode = u;
            if (u == endNode) {
                mazeState = MazeState.SOLVED;
                Invalidate();
                return;
            }

            if (AreAllNodesVisited()) {
                return;
            }

            visited[u] = true;

            // for each neighbor of u make a move
            List<int> neighbors = GetNeighbors(u);
            for (int i = 0; i < neighbors.Count; ++i) {
                int v = neighbors[i];
                if (adjacentMatrix[u, v] == true || adjacentMatrix[v, u] == true) {
                    path[v] = u;
                    DepthFirstSearch(v);
                    Thread.Sleep(10);
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs args) {
            Graphics g = args.Graphics;
            if (mazeState != MazeState.NOT_START) {
                if (currentSquare != null)
                    currentSquare.DrawTrace(g);

                if (maze != null) {
                    for (int r = 0; r < maze.Height; ++r) {
                        for (int c = 0; c < maze.Width; ++c) {
                            maze.SquareAt(r, c).Draw(g);
                        }
                    }
                }

                if (mazeState != MazeState.SOLVED && currentNode != -1) {
                    Point current = ConvertToPoint(currentNode);
                    g.FillRectangle(Brushes.Violet, current.X * maze.Spacing, current.Y * maze.Spacing, maze.Spacing, maze.Spacing);

                    for (int n = 0; n < nodes; ++n) {
                        if (visited[n]) {
                            Point trace = ConvertToPoint(n);
                            g.FillRectangle(Brushes.Tomato, trace.X * maze.Spacing, trace.Y * maze.Spacing, maze.Spacing, maze.Spacing);
                        }
                    }
                }

                if (mazeState == MazeState.SOLVED) {
                    DrawPath(g);
                }

                DrawStartAndEnd(g);
            }
        }

        private void DrawPath(Graphics g) {
            int u = endNode;
            int cnt = 0;
            while (path[u] != startNode) {
                Point point = ConvertToPoint(path[u]);
                g.DrawString(cnt.ToString(), font, Brushes.Red, new Point(point.X * maze.Spacing + maze.Spacing / 3, point.Y * maze.Spacing + maze.Spacing / 3));
                u = path[u];
                cnt++;
            }
        }

        private void DrawStartAndEnd(Graphics g) {
            if (startNode != -1) {
                Point start = ConvertToPoint(startNode);
                g.FillRectangle(Brushes.Green, start.X * maze.Spacing, start.Y * maze.Spacing, maze.Spacing, maze.Spacing);
            }
            if (endNode != -1) {
                Point end = ConvertToPoint(endNode);
                g.FillRectangle(Brushes.Yellow, end.X * maze.Spacing, end.Y * maze.Spacing, maze.Spacing, maze.Spacing);
            }
        }

        private List<int> GetNeighbors(int u) {
            List<int> neighbors = new List<int>();
            Point from = ConvertToPoint(u);
            int to;

            // up
            if (maze.IsInRange(from.Y - 1, from.X)) {
                to = ConvertToNode(from.Y - 1, from.X);
                if (!visited[to])
                    neighbors.Add(to);
            }

            // down 
            if (maze.IsInRange(from.Y + 1, from.X)) {
                to = ConvertToNode(from.Y + 1, from.X);
                if (!visited[to])
                    neighbors.Add(to);
            }

            // left 
            if (maze.IsInRange(from.Y, from.X - 1)) {
                to = ConvertToNode(from.Y, from.X - 1);
                if (!visited[to])
                    neighbors.Add(to);
            }

            // right 
            if (maze.IsInRange(from.Y, from.X + 1)) {
                to = ConvertToNode(from.Y, from.X + 1);
                if (!visited[to])
                    neighbors.Add(to);
            }

            return neighbors;
        }

        private void InitializeGraph() {
            nodes = maze.Width * maze.Height;
            visited = new bool[nodes];
            adjacentMatrix = new bool[nodes, nodes];
            path = new int[nodes];

            for (int u = 0; u < nodes; ++u) {
                visited[u] = false;
            }

            for (int u = 0; u < maze.Height; ++u) {
                for (int v = 0; v < maze.Width; ++v) {
                    adjacentMatrix[u, v] = false;
                    adjacentMatrix[v, u] = false;
                }
            }
        }

        private bool AreAllNodesVisited() {
            for (int i = 0; i < nodes; ++i) {
                if (!visited[i])
                    return false;
            }
            return true;
        }

        public int ConvertToNode(int r, int c) {
            return (r * maze.Height + c);
        }

        public Point ConvertToPoint(int v) {
            return new Point(v % maze.Width, v / maze.Height);
        }
    }
}
