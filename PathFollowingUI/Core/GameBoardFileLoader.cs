using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PathFollowingUI.Core
{
    /// <summary>
    /// Provides functionality to load and parse game board files.
    /// Handles validation, normalization, and conversion to the game board definition structure.
    /// </summary>
    public class GameBoardFileLoader
    {
        /// <summary>
        /// Character used to fill empty spaces in the game board.
        /// </summary>
        private const char EMPTY_CHAR = ' ';

        /// <summary>
        /// Set of valid characters allowed in the game board.
        /// </summary>
        private readonly string validBoardCharacters;

        /// <summary>
        /// Character representing the player's starting position.
        /// </summary>
        private readonly char playerStartPositionChar;

        /// <summary>
        /// Character representing the player's ending position.
        /// </summary>
        private readonly char playerEndPositionChar;

        /// <summary>
        /// Initializes a new instance of the GameBoardFileLoader class.
        /// </summary>
        /// <param name="validBoardCharacters">Set of valid characters in the game board</param>
        /// <param name="playerStart">Character representing player start position</param>
        /// <param name="playerEnd">Character representing player end position</param>
        public GameBoardFileLoader(string validBoardCharacters, char playerStart, char playerEnd)
        {
            // Include the empty space character in valid characters
            this.validBoardCharacters = validBoardCharacters + EMPTY_CHAR;

            playerStartPositionChar = playerStart;
            playerEndPositionChar = playerEnd;
        }

        /// <summary>
        /// Loads and processes a game board file, returning a fully initialized game board definition.
        /// </summary>
        /// <param name="path">Path to the game board file</param>
        /// <returns>A GameBoardDefinition populated with the game board data</returns>
        /// <exception cref="ArgumentException">Thrown when the file is invalid or missing</exception>
        public GameBoardDefinition LoadGameBoard(string path)
        {
            // Validate the file path
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new ArgumentException(Constants.ErrorFileNotFound, nameof(path));

            // Load all lines from the file
            var lines = File.ReadAllLines(path);

            // Check if the file has content
            if (lines.Length < 1)
                throw new ArgumentException(Constants.ErrorFileInvalid, nameof(path));

            // Find the length of the longest line for normalization
            int longestLine = lines.Select(l => l.Length).Max();

            // Normalize lines to ensure consistent width
            NormalizeBoardLines(longestLine, lines);

            // Validate the game board content
            ValidateGameBoard(lines);

            // Parse the normalized board into a GameBoardDefinition
            return GetGameBoardDefinition(lines);
        }

        /// <summary>
        /// Parses normalized game board lines into a GameBoardDefinition object.
        /// </summary>
        /// <param name="lines">Normalized lines representing the game board</param>
        /// <returns>A fully populated GameBoardDefinition</returns>
        private GameBoardDefinition GetGameBoardDefinition(string[] lines)
        {
            // Initialize positions (will be set during parsing)
            Point playerStartPos = new Point();
            Point playerEndPos = new Point();

            // Determine board dimensions
            int boardWidth = lines[0].Length;
            int boardHeight = lines.Length;

            // Initialize game board arrays
            char[,] gameBoard = new char[boardWidth, boardHeight];
            bool[,] solution = new bool[boardWidth, boardHeight];   // Defaults to false

            // Parse each character in the board
            for (var y = 0; y < boardHeight; y++)
            {
                string line = lines[y];

                for (var x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    // Record special positions
                    if (c == playerStartPositionChar)
                    {
                        playerStartPos = new Point(x, y);
                    }
                    else if (c == playerEndPositionChar)
                    {
                        playerEndPos = new Point(x, y);
                    }

                    // Store the character in the game board
                    gameBoard[x, y] = c;
                }
            }

            // Create and return the game board definition
            return new GameBoardDefinition
            {
                RawFile = lines,
                BoardData = gameBoard,
                BoardSolution = solution,
                BoardWidth = boardWidth,
                BoardHeight = boardHeight,
                PlayerStartPosition = playerStartPos,
                PlayerEndPosition = playerEndPos
            };
        }

        /// <summary>
        /// Validates the game board for allowed characters and required elements.
        /// </summary>
        /// <param name="lines">String array representing the game board</param>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        private void ValidateGameBoard(string[] lines)
        {
            // Helper function to check for valid characters
            void EnsureLineHasValidCharacters(string line)
            {
                foreach (var ch in line)
                {
                    if (!validBoardCharacters.Contains(ch))
                    {
                        string errorMessage = string.Format(Constants.ErrorLineHasInvalidCharacters, ch);
                        throw new ArgumentException(errorMessage, nameof(line));
                    }
                }
            }

            // Track whether required positions are found
            bool playerHasStartPosition = false;
            bool playerHasEndPosition = false;

            // Check each line for valid characters and special positions
            foreach (var line in lines)
            {
                // Validate characters
                EnsureLineHasValidCharacters(line);

                // Check for special positions
                foreach (var ch in line)
                {
                    if (ch == playerStartPositionChar)
                    {
                        // Ensure there's only one start position
                        if (playerHasStartPosition)
                        {
                            throw new ArgumentException(Constants.ErrorPlayerStartPosDuplicate, nameof(lines));
                        }

                        playerHasStartPosition = true;
                    }
                    else if (ch == playerEndPositionChar)
                    {
                        // Ensure there's only one end position
                        if (playerHasEndPosition)
                        {
                            throw new ArgumentException(Constants.ErrorPlayerEndPosDuplicate, nameof(lines));
                        }

                        playerHasEndPosition = true;
                    }
                }
            }

            // Ensure required positions are present
            if (!playerHasStartPosition)
            {
                throw new ArgumentException(Constants.ErrorPlayerStartPosNotFound, nameof(lines));
            }

            if (!playerHasEndPosition)
            {
                throw new ArgumentException(Constants.ErrorPlayerEndPosNotFound, nameof(lines));
            }
        }

        /// <summary>
        /// Normalizes board lines by padding shorter lines with spaces to make all lines the same length.
        /// </summary>
        /// <param name="length">Target length for all lines</param>
        /// <param name="lines">Reference to the string array to normalize</param>
        private void NormalizeBoardLines(int length, string[] lines)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int paddingNeeded = length - line.Length;

                // Add padding if the line is shorter than the target length
                if (paddingNeeded > 0)
                {
                    line += new string(EMPTY_CHAR, paddingNeeded);
                    lines[i] = line;
                }
            }
        }
    }
}
