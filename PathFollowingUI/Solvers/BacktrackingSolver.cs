using PathFollowingUI.Core;
using System.Drawing;
using System.Linq;

namespace PathFollowingUI.Solvers
{
    /// <summary>
    /// Provides functionality for solving path following problems using a backtracking algorithm.
    /// </summary>
    public class BacktrackingSolver
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
        /// Reference to the game board definition.
        /// </summary>
        private readonly GameBoardDefinition boardDefinition;

        /// <summary>
        /// Gets the actual game board data.
        /// </summary>
        private char[,] GameBoard => boardDefinition.BoardData;

        /// <summary>
        /// Gets the game board solution state.
        /// </summary>
        private bool[,] Solution => boardDefinition.BoardSolution;

        /// <summary>
        /// Gets the game board tunnels information.
        /// </summary>
        private bool[,] Tunnels => boardDefinition.BoardTunnels;

        /// <summary>
        /// Gets the player's ending position on the board.
        /// </summary>
        private Point PlayerEndPos => boardDefinition.PlayerEndPosition;

        /// <summary>
        /// Gets or sets the target word to be formed for the board solution.
        /// </summary>
        private string BoardWord { get; set; }

        /// <summary>
        /// Stores the solution word once found.
        /// </summary>
        private string solutionWord;

        /// <summary>
        /// Stores the path word representing the solution path once found.
        /// </summary>
        private string solutionPathWord;

        /// <summary>
        /// Initializes a new instance of the BacktrackingSolver class.
        /// </summary>
        /// <param name="boardDefinition">The game board definition to solve</param>
        public BacktrackingSolver(GameBoardDefinition boardDefinition)
        {
            this.boardDefinition = boardDefinition;
        }

        /// <summary>
        /// Checks if the given coordinates are within the game board boundaries.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if coordinates are valid, false otherwise</returns>
        private bool AreValidBounds(int x, int y)
        {
            return Utils.AreValidBounds(x, y, boardDefinition.BoardWidth, boardDefinition.BoardHeight);
        }

        /// <summary>
        /// Validates if a position is a valid next move in the path solution.
        /// </summary>
        /// <param name="x">X coordinate to check</param>
        /// <param name="y">Y coordinate to check</param>
        /// <param name="currentWord">Current accumulated word</param>
        /// <returns>True if the position is valid for the next move, false otherwise</returns>
        private bool IsValidPosition(int x, int y, string currentWord)
        {
            // Ensure coordinates are within board boundaries
            if (!AreValidBounds(x, y))
                return false;

            // Ensure position contains a valid board character
            if (!VALID_BOARD_CHARS.Contains(GameBoard[x, y]))
                return false;

            // Avoid revisiting positions unless it's a tunnel
            if (Solution[x, y] && !Tunnels[x, y])
                return false;

            // Ensure the move keeps us on track toward the solution word
            if (!BoardWord.StartsWith(currentWord))
                return false;

            return true;
        }

        /// <summary>
        /// Recursive method that tries to find a path solution using backtracking.
        /// </summary>
        /// <param name="x">Current X position</param>
        /// <param name="y">Current Y position</param>
        /// <param name="prevMovement">Previous movement character</param>
        /// <param name="currentWord">Current accumulated solution word</param>
        /// <param name="pathWord">Current path representation</param>
        /// <returns>True if a solution is found, false otherwise</returns>
        private bool MoveNextStep(int x, int y, char prevMovement, string currentWord, string pathWord)
        {
            // Notify listeners about the current position
            PlayerPositionChanged?.Invoke(boardDefinition, currentWord, pathWord);

            // Extract only alphabet characters from the current word
            string filteredWord = new string(currentWord.Where(c => ALPHABET.Contains(c)).ToArray());

            // Check if we've found a solution
            if (filteredWord == BoardWord)
            {
                if (x == PlayerEndPos.X && y == PlayerEndPos.Y)
                {
                    Solution[x, y] = true;
                    pathWord += GameBoard[x, y];
                    
                    // Store solution information
                    solutionWord = currentWord;
                    solutionPathWord = pathWord;

                    return true;
                }
            }

            char currentChar = ' ';

            // Determine the current character for movement decision
            if (AreValidBounds(x, y))
            {
                currentChar = GameBoard[x, y];
                if (ALPHABET.Contains(currentChar))
                    currentChar = '+';

                if (Solution[x, y] && Tunnels[x, y])
                    currentChar = prevMovement;
            }

            // Check if this is a valid position to move into
            if (IsValidPosition(x, y, filteredWord))
            {
                // Handle tunnels
                if (Solution[x, y])
                    currentChar = prevMovement;

                // Update path and word
                pathWord += GameBoard[x, y];
                
                // Add to current word if it's a letter
                if (ALPHABET.Contains(GameBoard[x, y]) && !Solution[x, y])
                    currentWord += GameBoard[x, y];

                // Mark position as visited
                Solution[x, y] = true;

                // Try movement in directions based on the current character
                bool solutionFound = TryMovementDirections(x, y, currentChar, currentWord, pathWord);
                if (solutionFound)
                    return true;

                // Backtrack: if no solution found, undo changes
                if (!Tunnels[x, y])
                    Solution[x, y] = false;

                // Remove the last character from the word and path
                currentWord = currentWord.Length > 0 ? currentWord.Substring(0, currentWord.Length - 1) : "";
                pathWord = pathWord.Length > 0 ? pathWord.Substring(0, pathWord.Length - 1) : "";
            }

            return false;
        }

        /// <summary>
        /// Attempts to move in valid directions based on the current character.
        /// </summary>
        /// <param name="x">Current X position</param>
        /// <param name="y">Current Y position</param>
        /// <param name="currentChar">Current character determining movement options</param>
        /// <param name="currentWord">Current accumulated solution word</param>
        /// <param name="pathWord">Current path representation</param>
        /// <returns>True if a solution is found in any direction, false otherwise</returns>
        private bool TryMovementDirections(int x, int y, char currentChar, string currentWord, string pathWord)
        {
            switch (currentChar)
            {
                case (char)BoardLetterDefinition.MoveHorizontal:
                    // Try horizontal movement only
                    if (MoveNextStep(x - 1, y, currentChar, currentWord, pathWord))
                        return true;
                    if (MoveNextStep(x + 1, y, currentChar, currentWord, pathWord))
                        return true;
                    break;

                case (char)BoardLetterDefinition.MoveVertical:
                    // Try vertical movement only
                    if (MoveNextStep(x, y - 1, currentChar, currentWord, pathWord))
                        return true;
                    if (MoveNextStep(x, y + 1, currentChar, currentWord, pathWord))
                        return true;
                    break;

                case (char)BoardLetterDefinition.MoveAnywhere:
                case (char)BoardLetterDefinition.PlayerStartPosition:
                    // Try all four directions
                    if (MoveNextStep(x + 1, y, currentChar, currentWord, pathWord)) return true;
                    if (MoveNextStep(x, y + 1, currentChar, currentWord, pathWord)) return true;
                    if (MoveNextStep(x - 1, y, currentChar, currentWord, pathWord)) return true;
                    if (MoveNextStep(x, y - 1, currentChar, currentWord, pathWord)) return true;
                    break;
                
                default:
                    // This handles the '+' case (representing letters where any direction is allowed)
                    if (currentChar == '+')
                    {
                        if (MoveNextStep(x + 1, y, currentChar, currentWord, pathWord)) return true;
                        if (MoveNextStep(x, y + 1, currentChar, currentWord, pathWord)) return true;
                        if (MoveNextStep(x - 1, y, currentChar, currentWord, pathWord)) return true;
                        if (MoveNextStep(x, y - 1, currentChar, currentWord, pathWord)) return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Attempts to find a path solution for the game board.
        /// </summary>
        /// <param name="x">Starting X position</param>
        /// <param name="y">Starting Y position</param>
        /// <param name="boardWordSolution">Target word to form</param>
        /// <param name="solution">Reference to the solution grid</param>
        /// <returns>A tuple containing solution status and information</returns>
        public (bool SolutionFound, string SolutionWord, string PathWord) FindSolution(
            int x, int y, string boardWordSolution, bool[,] solution)
        {
            // Note: The solution grid is already accessible through the Solution property
            // which points to boardDefinition.BoardSolution
            BoardWord = boardWordSolution;

            // Start the recursive solving process
            bool found = MoveNextStep(x, y, ' ', "", "");

            return (SolutionFound: found, SolutionWord: solutionWord, PathWord: solutionPathWord);
        }
    }
}
