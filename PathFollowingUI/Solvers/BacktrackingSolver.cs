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
            if (filteredWord == BoardWord && x == PlayerEndPos.X && y == PlayerEndPos.Y)
            {
                Solution[x, y] = true;
                pathWord += GameBoard[x, y];
                
                // Store solution information
                solutionWord = currentWord;
                solutionPathWord = pathWord;

                return true;
            }

            // If position is invalid, return false immediately
            if (!IsValidPosition(x, y, filteredWord))
                return false;

            // Get the character at this position
            char cellChar = GameBoard[x, y];

            // Determine movement character for this cell
            char movementChar = DetermineMovementChar(x, y, prevMovement);
            
            // Update path and temporary word
            string newPathWord = pathWord + cellChar;
            string newCurrentWord = currentWord;
            
            // Add to current word if it's a letter and not already part of solution
            if (ALPHABET.Contains(cellChar) && !Solution[x, y])
                newCurrentWord += cellChar;

            // Mark position as visited
            Solution[x, y] = true;

            // Try movement in directions based on the current character
            bool solutionFound = TryMovementDirections(x, y, movementChar, newCurrentWord, newPathWord);
            
            // If solution found, return success
            if (solutionFound)
                return true;

            // Backtrack: if no solution found, undo changes
            if (!Tunnels[x, y])
                Solution[x, y] = false;

            // No solution found in this path
            return false;
        }
        
        /// <summary>
        /// Determines the appropriate movement character for the current cell
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="prevMovement">Previous movement character</param>
        /// <returns>Character determining movement options</returns>
        private char DetermineMovementChar(int x, int y, char prevMovement)
        {
            if (!AreValidBounds(x, y))
                return ' ';
                
            char cellChar = GameBoard[x, y];
            
            // If it's an alphabet character, treat it as allowing movement in all directions
            if (ALPHABET.Contains(cellChar))
                return '+';
                
            // For tunnels, preserve the previous movement direction
            if (Solution[x, y] && Tunnels[x, y])
                return prevMovement;
                
            return cellChar;
        }

        /// <summary>
        /// Direction vectors for movement: right, down, left, up
        /// </summary>
        private readonly (int dx, int dy)[] AllDirections = { (1, 0), (0, 1), (-1, 0), (0, -1) };
        
        /// <summary>
        /// Direction vectors for horizontal movement: left, right
        /// </summary>
        private readonly (int dx, int dy)[] HorizontalDirections = { (-1, 0), (1, 0) };
        
        /// <summary>
        /// Direction vectors for vertical movement: up, down
        /// </summary>
        private readonly (int dx, int dy)[] VerticalDirections = { (0, -1), (0, 1) };

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
            // Choose direction vectors based on the current character
            (int dx, int dy)[] directions;
            
            switch (currentChar)
            {
                case (char)BoardLetterDefinition.MoveHorizontal:
                    directions = HorizontalDirections;
                    break;
                    
                case (char)BoardLetterDefinition.MoveVertical:
                    directions = VerticalDirections;
                    break;
                    
                case (char)BoardLetterDefinition.MoveAnywhere:
                case (char)BoardLetterDefinition.PlayerStartPosition:
                    directions = AllDirections;
                    break;
                    
                default:
                    // Handle '+' (alphabet characters) or any other character
                    directions = currentChar == '+' ? AllDirections : null;
                    
                    // Exit if there's no valid movement direction
                    if (directions == null)
                        return false;
                    break;
            }
            
            // Try all applicable directions
            foreach (var (dx, dy) in directions)
            {
                if (MoveNextStep(x + dx, y + dy, currentChar, currentWord, pathWord))
                    return true;
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
