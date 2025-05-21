using PathFollowingUI.Core;
using PathFollowingUI.Solvers;
using System;
using System.Drawing;
using System.IO;

namespace PathFollowingUI
{
    /// <summary>
    /// Provides functionality for solving path following problems.
    /// Acts as a facade for the underlying solving algorithm.
    /// </summary>
    public class PathFollowingSolver
    {
        /// <summary>
        /// Event that triggers when player position changes during solving.
        /// </summary>
        public event CustomEvents.PlayerPositionChangedHandler PlayerPositionChanged;

        /// <summary>
        /// Defines valid letters on a game board.
        /// </summary>
        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Defines valid characters on a game board.
        /// </summary>
        private readonly string VALID_BOARD_CHARS = ALPHABET +
            (char)BoardLetterDefinition.MoveAnywhere +
            (char)BoardLetterDefinition.MoveHorizontal +
            (char)BoardLetterDefinition.MoveVertical +
            (char)BoardLetterDefinition.PlayerEndPosition +
            (char)BoardLetterDefinition.PlayerStartPosition;

        /// <summary>
        /// Stores game board information (board definition, found solution, etc.)
        /// </summary>
        private GameBoardDefinition boardDefinition;

        /// <summary>
        /// Helper Utility that loads Game Board from file.
        /// </summary>
        private GameBoardFileLoader boardLoader;

        /// <summary>
        /// Gets the board loader, creating it if necessary (lazy loading).
        /// </summary>
        private GameBoardFileLoader BoardLoader
        {
            get
            {
                // Lazy load the board loader when first needed
                if (boardLoader == null)
                    boardLoader = new GameBoardFileLoader(
                        VALID_BOARD_CHARS,
                        (char)BoardLetterDefinition.PlayerStartPosition,
                        (char)BoardLetterDefinition.PlayerEndPosition);

                return boardLoader;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PathFollowingSolver class.
        /// </summary>
        /// <param name="gameBoardPath">Full path to the game board file (e.g. c:\temp\board1.txt)</param>
        /// <param name="endSolution">Target solution word (e.g. ABCD)</param>
        /// <exception cref="ArgumentException">Thrown when inputs are invalid</exception>
        public PathFollowingSolver(string gameBoardPath, string endSolution)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(gameBoardPath) || !File.Exists(gameBoardPath))
                throw new ArgumentException(Constants.ErrorFileNotFound, nameof(gameBoardPath));

            if (string.IsNullOrWhiteSpace(endSolution))
                throw new ArgumentException(Constants.ErrorValueIsEmpty, nameof(endSolution));

            // Load game board
            LoadBoard(gameBoardPath, endSolution);
        }

        /// <summary>
        /// Attempts to solve the game board using the backtracking algorithm.
        /// </summary>
        /// <param name="solutionWord">When method returns, contains the found solution word or empty if no solution</param>
        /// <param name="pathWord">When method returns, contains the path word or empty if no solution</param>
        /// <returns>True if a solution was found, false otherwise</returns>
        public bool Solve(out string solutionWord, out string pathWord)
        {
            // Create a solver instance using the backtracking algorithm
            BacktrackingSolver solver = new BacktrackingSolver(boardDefinition);

            // Register for position change events if needed
            if (PlayerPositionChanged != null)
            {
                solver.PlayerPositionChanged += Solver_PlayerPositionChanged;
            }

            // Get the starting position
            Point startPos = boardDefinition.PlayerStartPosition;

            // Attempt to find a solution
            var solution = solver.FindSolution(
                startPos.X,
                startPos.Y,
                boardDefinition.BoardWordSolution,
                boardDefinition.BoardSolution);

            // Extract solution details
            bool solutionFound = solution.SolutionFound;
            solutionWord = solution.SolutionWord;
            pathWord = solution.PathWord;

            return solutionFound;
        }

        /// <summary>
        /// Event handler that forwards player position changes to subscribers.
        /// </summary>
        private void Solver_PlayerPositionChanged(GameBoardDefinition gameBoard, string currentWord, string pathWord)
        {
            PlayerPositionChanged(gameBoard, currentWord, pathWord);
        }

        /// <summary>
        /// Initializes the game board and sets starting/ending player positions.
        /// </summary>
        /// <param name="gameBoardPath">Full path to the game board file</param>
        /// <param name="endSolution">Target solution word</param>
        private void LoadBoard(string gameBoardPath, string endSolution)
        {
            // Load the game board from file
            boardDefinition = BoardLoader.LoadGameBoard(gameBoardPath);

            // Set additional properties
            boardDefinition.BoardWordSolution = endSolution;
            boardDefinition.BoardTunnels = FindTunnels(boardDefinition);
        }

        /// <summary>
        /// Finds "tunnels" in a game board, where vertical path solution can cross horizontal path solution, and vice versa.
        /// </summary>
        /// <param name="boardDefinition">Current game board definition</param>
        /// <returns>A 2D array indicating tunnel positions</returns>
        private bool[,] FindTunnels(GameBoardDefinition boardDefinition)
        {
            int boardWidth = boardDefinition.BoardWidth;
            int boardHeight = boardDefinition.BoardHeight;
            char[,] gameBoard = boardDefinition.BoardData;

            bool[,] tunnels = new bool[boardWidth, boardHeight];

            // Helper method to check if a position can be a tunnel based on surrounding cells
            bool AreTunnelBoundsValid(int x, int y, bool checkVertical)
            {
                if (checkVertical)
                {
                    return AreValidBounds(x, y - 1, boardDefinition) &&
                           AreValidBounds(x, y + 1, boardDefinition) &&
                           gameBoard[x, y - 1] == (char)BoardLetterDefinition.MoveVertical &&
                           gameBoard[x, y + 1] == (char)BoardLetterDefinition.MoveVertical;
                }
                else
                {
                    return AreValidBounds(x - 1, y, boardDefinition) &&
                           AreValidBounds(x + 1, y, boardDefinition) &&
                           gameBoard[x - 1, y] == (char)BoardLetterDefinition.MoveHorizontal &&
                           gameBoard[x + 1, y] == (char)BoardLetterDefinition.MoveHorizontal;
                }
            }

            // Scan the board to identify tunnel positions
            for (var y = 0; y < boardHeight; y++)
            {
                for (var x = 0; x < boardWidth; x++)
                {
                    char c = gameBoard[x, y];

                    if (c == (char)BoardLetterDefinition.MoveHorizontal ||
                        c == (char)BoardLetterDefinition.MoveVertical)
                    {
                        switch (c)
                        {
                            case (char)BoardLetterDefinition.MoveHorizontal:
                                // Horizontal paths can intersect with vertical paths
                                if (AreTunnelBoundsValid(x, y, true))
                                {
                                    tunnels[x, y] = true;
                                }
                                break;

                            case (char)BoardLetterDefinition.MoveVertical:
                                // Vertical paths can intersect with horizontal paths
                                if (AreTunnelBoundsValid(x, y, false))
                                {
                                    tunnels[x, y] = true;
                                }
                                break;
                        }
                    }
                    else if (ALPHABET.Contains(c))
                    {
                        // Letters can be tunnels if they're at intersections
                        if (AreTunnelBoundsValid(x, y, true) && AreTunnelBoundsValid(x, y, false))
                        {
                            tunnels[x, y] = true;
                        }
                    }
                }
            }

            return tunnels;
        }

        /// <summary>
        /// Checks if the given coordinates are within the game board boundaries.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="boardDefinition">Game board definition</param>
        /// <returns>True if coordinates are valid, false otherwise</returns>
        private bool AreValidBounds(int x, int y, GameBoardDefinition boardDefinition)
        {
            return Utils.AreValidBounds(x, y, boardDefinition.BoardWidth, boardDefinition.BoardHeight);
        }
    }
}
