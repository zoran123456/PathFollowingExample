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
        /// Performs several checks to determine if this position can be part of the solution path.
        /// </summary>
        /// <param name="x">X coordinate to check</param>
        /// <param name="y">Y coordinate to check</param>
        /// <param name="currentWord">Current accumulated word</param>
        /// <returns>True if the position is valid for the next move, false otherwise</returns>
        private bool IsValidPosition(int x, int y, string currentWord)
        {
            // VALIDATION 1: Ensure coordinates are within board boundaries
            if (!AreValidBounds(x, y))
                return false;

            // VALIDATION 2: Ensure position contains a valid board character
            // (Must be a letter, movement symbol, or player position marker)
            if (!VALID_BOARD_CHARS.Contains(GameBoard[x, y]))
                return false;

            // VALIDATION 3: Avoid revisiting positions unless it's a tunnel
            // (This prevents endless loops in the solution path)
            if (Solution[x, y] && !Tunnels[x, y])
                return false;

            // VALIDATION 4: Ensure the move keeps us on track toward the solution word
            // (The current word must be a valid prefix of the target word)
            if (!BoardWord.StartsWith(currentWord))
                return false;

            // All validations passed, position is valid for the next move
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
            // STEP 1: Notify listeners about the current position (for UI updates)
            PlayerPositionChanged?.Invoke(boardDefinition, currentWord, pathWord);

            // STEP 2: Calculate the filtered word (letters only) for comparison with target
            string filteredWord = new string(currentWord.Where(c => ALPHABET.Contains(c)).ToArray());

            // STEP 3: Check if we've reached the target word and ending position
            if (filteredWord == BoardWord && x == PlayerEndPos.X && y == PlayerEndPos.Y)
            {
                // SUCCESS: Solution found
                Solution[x, y] = true;
                pathWord += GameBoard[x, y];
                
                // Store complete solution information
                solutionWord = currentWord;
                solutionPathWord = pathWord;

                return true;
            }

            // STEP 4: Early validation - quickly reject invalid positions
            if (!IsValidPosition(x, y, filteredWord))
                return false;

            // STEP 5: Get the character at current cell and determine movement type
            char cellChar = GameBoard[x, y];
            char movementChar = DetermineMovementChar(x, y, prevMovement);
            
            // STEP 6: Update path tracking for this step
            string newPathWord = pathWord + cellChar;
            string newCurrentWord = currentWord;
            
            // STEP 7: If this is a letter cell, add it to the building word
            if (ALPHABET.Contains(cellChar) && !Solution[x, y])
                newCurrentWord += cellChar;

            // STEP 8: Mark this position as visited in the solution grid
            Solution[x, y] = true;

            // STEP 9: Try exploring in valid directions from here
            bool solutionFound = TryMovementDirections(x, y, movementChar, newCurrentWord, newPathWord);
            
            // STEP 10: If solution found in any direction, propagate success upward
            if (solutionFound)
                return true;

            // STEP 11: Backtrack - undo the visit to this cell (unless it's a tunnel)
            if (!Tunnels[x, y])
                Solution[x, y] = false;

            // No solution found from this position
            return false;
        }
        
        /// <summary>
        /// Determines the appropriate movement character for the current cell.
        /// This method centralizes the logic for determining which movement rules apply
        /// based on the current cell type, whether it's a tunnel, or a letter.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="prevMovement">Previous movement character</param>
        /// <returns>Character determining movement options</returns>
        private char DetermineMovementChar(int x, int y, char prevMovement)
        {
            // Safety check for bounds
            if (!AreValidBounds(x, y))
                return ' ';
                
            char cellChar = GameBoard[x, y];
            
            // Special handling for different cell types:
            
            // 1. For alphabet characters, treat them as allowing movement in all directions
            if (ALPHABET.Contains(cellChar))
                return '+';
                
            // 2. For tunnels, preserve the previous movement direction to continue through
            if (Solution[x, y] && Tunnels[x, y])
                return prevMovement;
                
            // 3. Otherwise, use the character at the cell (-, |, +, etc.)
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
            // STEP 1: Select appropriate direction vectors based on the current character type
            (int dx, int dy)[] directions;
            
            switch (currentChar)
            {
                case (char)BoardLetterDefinition.MoveHorizontal:
                    // For horizontal movement tiles, try left and right only
                    directions = HorizontalDirections;
                    break;
                    
                case (char)BoardLetterDefinition.MoveVertical:
                    // For vertical movement tiles, try up and down only
                    directions = VerticalDirections;
                    break;
                    
                case (char)BoardLetterDefinition.MoveAnywhere:
                case (char)BoardLetterDefinition.PlayerStartPosition:
                    // For "anywhere" movement tiles and the start position, try all four directions
                    directions = AllDirections;
                    break;
                    
                default:
                    // Special case for '+' (alphabet characters) or handling other characters
                    directions = currentChar == '+' ? AllDirections : null;
                    
                    // Skip exploration if there are no valid directions to try
                    if (directions == null)
                        return false;
                    break;
            }
            
            // STEP 2: Try all applicable directions in order
            foreach (var (dx, dy) in directions)
            {
                // Recursively explore each direction, short-circuit if a solution is found
                if (MoveNextStep(x + dx, y + dy, currentChar, currentWord, pathWord))
                    return true;
            }
            
            // No solution found in any direction
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
