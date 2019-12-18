using PathFollowingUI.Core;
using PathFollowingUI.Solvers;
using System;
using System.Drawing;
using System.IO;

namespace PathFollowingUI
{

    /// <summary>
    /// Represents a class for solving path following problem
    /// </summary>
    public class PathFollowingSolver
    {
        public event CustomEvents.PlayerPositionChangedHandler PlayerPositionChanged;

        /// <summary>
        /// Defines valid letters on a game board
        /// </summary>
        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Defines valid characters on a game board
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
        /// Helper Utility that loads Game Board from file
        /// </summary>
        private GameBoardFileLoader boardLoader;
        private GameBoardFileLoader BoardLoader
        {
            get
            {
                // Lazy load class, a good practice to use it when possible

                if (boardLoader == null)
                    boardLoader = new GameBoardFileLoader(
                        VALID_BOARD_CHARS,
                        (char)BoardLetterDefinition.PlayerStartPosition,
                        (char)BoardLetterDefinition.PlayerEndPosition);

                return boardLoader;
            }
        }

        /// <summary>
        /// Initializes new Solver instance
        /// </summary>
        /// <param name="gameBoardPath">Points to a full path to game board (e.g. c:\\temp\\board1.txt)</param>
        /// <param name="endSolution">Solution of a board (e.g. ABCD)</param>
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
        /// Attempts to Solve game board using provided algorithm
        /// </summary>
        /// <returns></returns>
        public bool Solve(out string solutionWord, out string pathWord)
        {
            BacktrackingSolver solver = new BacktrackingSolver(boardDefinition);

            if (PlayerPositionChanged !=null)
            {
                solver.PlayerPositionChanged += Solver_PlayerPositionChanged;
            }

            Point startPos = boardDefinition.PlayerStartPosition;

            var solution = solver.FindSolution(
                startPos.X,
                startPos.Y,
                boardDefinition.BoardWordSolution,
                boardDefinition.BoardSolution);


            bool solutionFound = solution.SolutionFound;
            solutionWord = solution.SolutionWord;
            pathWord = solution.PathWord;

            return solutionFound;
        }

        private void Solver_PlayerPositionChanged(GameBoardDefinition gameBoard, string currentWord, string pathWord)
        {
            PlayerPositionChanged(gameBoard, currentWord, pathWord);
        }

        /// <summary>
        /// Initializes game board and sets starting/ending player position
        /// </summary>
        /// <param name="gameBoardPath">Location of a full path to a game board</param>
        /// <param name="endSolution">Solution of a board (e.g. ABCD)</param>
        private void LoadBoard(string gameBoardPath, string endSolution)
        {
            // Initialize game board, player start/end position and set board solution

            boardDefinition = BoardLoader.LoadGameBoard(gameBoardPath);

            // Set properties not populated with LoadGameBoard method
            boardDefinition.BoardWordSolution = endSolution;
            boardDefinition.BoardTunnels = FindTunnels(boardDefinition);
        }

        /// <summary>
        /// Finds "tunnels" in a game board, vertical path solution can cross horizontal path solution, and vice versa
        /// </summary>
        /// <param name="boardDefinition"></param>
        /// <returns></returns>
        private bool[,] FindTunnels(GameBoardDefinition boardDefinition)
        {
            int boardWidth = boardDefinition.BoardWidth;
            int boardHeight = boardDefinition.BoardHeight;
            char[,] gameBoard = boardDefinition.BoardData;

            bool[,] tunnels = new bool[boardWidth, boardHeight];

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
                                if (AreTunnelBoundsValid(x, y, true))
                                {
                                    tunnels[x, y] = true;
                                }
                                break;

                            case (char)BoardLetterDefinition.MoveVertical:
                                if (AreTunnelBoundsValid(x, y, false))
                                {
                                    tunnels[x, y] = true;
                                }
                                break;
                        }
                    }
                    else if (ALPHABET.Contains(c))
                    {
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
        /// Determines whether current X,Y position is withing game board bounds
        /// </summary>
        private bool AreValidBounds(int x, int y, GameBoardDefinition boardDefinition)
        {
            return Utils.AreValidBounds(x, y, boardDefinition.BoardWidth, boardDefinition.BoardHeight);
        }
    }

}
