using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace WallMaze {
    public class Maze {
        private int width;
        private int height;
        private int spacing;
        private Square[,] squares;
        private List<Square> randomUnblockedSquares;
        private Random random;

        public Maze(int w, int h, int s) {
            width = w;
            height = h;
            spacing = s;

            random = new Random();

            List<int> randomList = CreateSloppyRandomList(width * height);
            randomUnblockedSquares = new List<Square>();

            // initialize all squares
            squares = new Square[height, width];
            int i = 0;
            for (int r = 0; r < height; ++r) {
                for (int c = 0; c < width; ++c) {
                    squares[r, c] = new Square(r, c, this);
                    squares[r, c].Id = randomList[i];
                    i++;
                }
            }
        }

        private List<int> CreateSloppyRandomList(int n) {
            List<int> randomList = new List<int>();
            for (int i = 0; i < n; ++i) {
                randomList.Add(i);
            }

            // shuffle
            for (int i = 0; i < randomList.Count; ++i) {
                int idx = random.Next(randomList.Count);
                int temp = randomList[i];
                randomList[i] = randomList[idx];
                randomList[idx] = temp;
            }

            return randomList;
        }

        /// <summary>
        /// Simple accessor that returns a reference to the square 
        /// in the grid at row r and column c.
        /// </summary>
        /// <param name="r">row</param>
        /// <param name="c">column</param>
        /// <returns></returns>
        public Square SquareAt(int r, int c) {
            if (IsInRange(r, c))
                return squares[r, c];

            return null;
        }

        /// <summary>
        /// Check if a position in grid
        /// is in valid range 
        /// </summary>
        /// <param name="r">row</param>
        /// <param name="c">column</param>
        /// <returns></returns>
        public bool IsInRange(int r, int c) {
            return (r >= 0 && r < height && c >= 0 && c < width);
        }
        
        /// <summary>
        /// Checks to see if any square in the 
        /// grid is blocked
        /// </summary>
        /// <returns></returns>
        public bool IsAnyBlocked() {
            for (int r = 0; r < height; ++r) {
                for (int c = 0; c < width; ++c) {
                    if (squares[r, c].IsBlocked())
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a randomly selected unblocked 
        /// square from somewhere in the grid of squares
        /// </summary>
        /// <returns></returns>
        public Square SelectRandomUnblockedSquare() {
            randomUnblockedSquares.Clear();
            for (int r = 0; r < height; ++r) {
                for (int c = 0; c < width; ++c) {
                    if (!squares[r, c].IsBlocked()) {
                        // get all adjacent if exist
                        Square adjacentLeft = squares[r, c].AdjacentSquare(Square.Direction.LEFT);
                        Square adjacentRight = squares[r, c].AdjacentSquare(Square.Direction.RIGHT);
                        Square adjacentTop = squares[r, c].AdjacentSquare(Square.Direction.TOP);
                        Square adjacentBottom = squares[r, c].AdjacentSquare(Square.Direction.BOTTOM);
                       
                        // ensuring at least one of them is blocked
                        if ((adjacentLeft != null && adjacentLeft.IsBlocked()) || (adjacentRight != null && adjacentRight.IsBlocked())  ||
                            (adjacentTop != null && adjacentTop.IsBlocked())   || (adjacentBottom != null && adjacentBottom.IsBlocked())) {
                            // add them to the list
                            randomUnblockedSquares.Add(squares[r, c]);
                        }
                    }
                }
            }

            if (randomUnblockedSquares.Count > 0) {
                return randomUnblockedSquares[random.Next(randomUnblockedSquares.Count)];
            }

            return null;
        }

        public int Width {
            get {
                return width;
            }
            set {
                width = value;
            }
        }

        public int Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }

        public Square[,] Squares {
            get {
                return squares;
            }
        }

        public int Spacing {
            get {
                return spacing;
            }
            set {
                spacing = value;
            }
        }

        public void Draw(Graphics g) {
            for (int r = 0; r < height; ++r) {
                for (int c = 0; c < width; ++c) {
                    squares[r, c].Draw(g);    
                }
            }
        }
    }
}
