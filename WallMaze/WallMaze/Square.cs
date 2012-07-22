using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WallMaze {
    public class Square : IComparable<Square> {
        public enum Direction {
            TOP,
            RIGHT,
            BOTTOM,
            LEFT
        };

        private int row;
        private int column;
        private int state;

        private bool left;
        private bool right;
        private bool bottom;
        private bool top;

        private static Maze maze;
        private List<Direction> directions;

        private static Random random;
        private static Pen pen;
        private static Pen removedPen;
        private static Pen nodePen;
        private int id;

        public Square(int r, int c, Maze m) {
            row = r;
            column = c;
            maze = m;

            // initially, all sides have wall
            left = true;
            right = true;
            bottom = true;
            top = true;

            random = new Random();
            directions = new List<Direction>();

            pen = new Pen(Brushes.Black);
            pen.Width = 3.0f;

            removedPen = new Pen(SystemColors.Control);
            removedPen.Width = 3.0f;

            nodePen = new Pen(Brushes.Red);
            nodePen.Width = 3.0f;
        }

        public int Id {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        /// <summary>
        /// This method returns true if the Square has the indicated 
        /// side (wall), false otherwise.
        /// </summary>
        /// <param name="d">side wall</param>
        /// <returns></returns>
        public bool HasSide(Direction d) {
            if (d == Direction.LEFT)
                return left;
            else if (d == Direction.RIGHT)
                return right;
            else if (d == Direction.TOP)
                return top;
            else  // (d == Direction.BOTTOM)
                return bottom;
        }

        /// <summary>
        /// Return trues if all sides exist (no side has been removed)
        /// </summary>
        /// <returns></returns>
        public bool IsBlocked() {
            return (left && right && top && bottom);
        }

        /// <summary>
        /// This method returns true if the Square is on the 
        /// indicated edge of the grid, false otherwise.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool HasEdge(Direction dir) {
            return false;
        }

        /// <summary>
        /// This method returns a reference to the Square that 
        /// is adjacent to the current Square in the indicated 
        /// direction (returns null if there is no adjacent square 
        /// in that direction)
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public Square AdjacentSquare(Direction dir) {
            switch (dir) {
                case Direction.LEFT:
                    if (maze.IsInRange(row, column - 1))
                        return maze.Squares[row, column - 1];
                    break;

                case Direction.RIGHT:
                    if (maze.IsInRange(row, column + 1))
                        return maze.Squares[row, column + 1];
                    break;

                case Direction.TOP:
                    if (maze.IsInRange(row - 1, column))
                        return maze.Squares[row - 1, column];
                    break;

                case Direction.BOTTOM:
                    if (maze.IsInRange(row + 1, column))
                        return maze.Squares[row + 1, column];
                    break;
            }

            return null;
        }



        public void RemoveSide(Direction dir) {
            switch (dir) {
                case Direction.LEFT:
                    left = false;
                    break;

                case Direction.RIGHT:
                    right = false;
                    break;

                case Direction.TOP:
                    top = false;
                    break;

                case Direction.BOTTOM:
                    bottom = false;
                    break;
            }
        }

        public static Direction OppositeSide(Direction dir) {
            Direction opposite = dir;
            switch (dir) {
                case Direction.LEFT:
                    opposite = Direction.RIGHT;
                    break;

                case Direction.RIGHT:
                    opposite = Direction.LEFT;
                    break;

                case Direction.TOP:
                    opposite = Direction.BOTTOM;
                    break;

                case Direction.BOTTOM:
                    opposite = Direction.TOP;
                    break;
            }

            return opposite;
        }

        public Direction SelectOpenSide() {
            directions.Clear();

            if (!left)
                directions.Add(Direction.LEFT);

            if (!right)
                directions.Add(Direction.RIGHT);

            if (!top)
                directions.Add(Direction.TOP);

            if (!bottom)
                directions.Add(Direction.BOTTOM);

            if (directions.Count > 0) {
                int i = random.Next(directions.Count);
                return directions[i];
            }

            // ???
            return Direction.LEFT;
        }

        public bool Left {
            get {
                return left;
            }
            set {
                left = value;
            }
        }

        public bool Right {
            get {
                return right;
            }
            set {
                right = value;
            }
        }

        public bool Bottom {
            get {
                return bottom;
            }
            set {
                bottom = value;
            }
        }

        public bool Top {
            get {
                return top;
            }
            set {
                top = value;
            }
        }

        public int Row {
            get {
                return row;
            }
        }

        public int Column {
            get {
                return column;
            }
        }

        public int State {
            get {
                return state;
            }
            set {
                state = value;
            }
        }

        public void Draw(Graphics g) {
            int r = row * maze.Spacing;
            int c = column * maze.Spacing;
            // top 
            if (top)
                g.DrawLine(pen, new Point(c, r), new Point(c + maze.Spacing, r));
            else
                g.DrawLine(removedPen, new Point(c, r), new Point(c + maze.Spacing, r));

            // left
            if (left)
                g.DrawLine(pen, new Point(c, r), new Point(c, r + maze.Spacing));
            else
                g.DrawLine(removedPen, new Point(c, r), new Point(c, r + maze.Spacing));

            // right
            if (right)
                g.DrawLine(pen, new Point(c + maze.Spacing, r), new Point(c + maze.Spacing, r + maze.Spacing));
            else
                g.DrawLine(removedPen, new Point(c + maze.Spacing, r), new Point(c + maze.Spacing, r + maze.Spacing));

            // bottom
            if (bottom)
                g.DrawLine(pen, new Point(c, r + maze.Spacing), new Point(c + maze.Spacing, r + maze.Spacing));
            else
                g.DrawLine(removedPen, new Point(c, r + maze.Spacing), new Point(c + maze.Spacing, r + maze.Spacing));
        }

        public void DrawTrace(Graphics g) {
            g.FillRectangle(Brushes.Red, column * maze.Spacing, row * maze.Spacing, maze.Spacing, maze.Spacing);
        }

        public int CompareTo(Square other) {
            if (id > other.id)
                return 1;
            else if (id < other.id)
                return -1;
            else
                return 0;
        }
    }
}
