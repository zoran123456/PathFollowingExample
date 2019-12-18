using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PathFollowingUI.Core
{
    /// <summary>
    /// Helper utility that loads unnormalized file and returns string array with canonical form
    /// </summary>
    public class GameBoardFileLoader
    {
        private const char EMPTY_CHAR = ' ';

        private readonly string validBoardCharacters;

        private readonly char playerStartPositionChar;

        private readonly char playerEndPositionChar;

        /// <summary>
        /// Initializes new instance of a class
        /// </summary>
        /// <param name="validBoardCharacters">Defines valid characters in a game board</param>
        /// <param name="playerStart">Defines character for player start position</param>
        /// <param name="playerEnd">Defines character for player end position</param>
        public GameBoardFileLoader(string validBoardCharacters, char playerStart, char playerEnd)
        {
            this.validBoardCharacters = validBoardCharacters + EMPTY_CHAR;

            playerStartPositionChar = playerStart;
            playerEndPositionChar = playerEnd;
        }

        /// <summary>
        /// Loads unnormalized file and returns string array with canonical form
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameBoardDefinition LoadGameBoard(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new ArgumentException(Constants.ErrorFileNotFound, nameof(path));

            var lines = File.ReadAllLines(path);

            // Check if file is empty
            if (lines.Length < 1)
                throw new ArgumentException(Constants.ErrorFileInvalid, nameof(path));

            // Find longest line
            int longestLine = lines.Select(l => l.Length).OrderByDescending(l => l).First();

            // Normalize lines (extrude lines shorter than longest line in an array)
            NormalizeBoardLines(longestLine, lines);

            // Validate game board for player start/end position and allowed characters
            ValidateGameBoard(lines);



            return GetGameBoardDefinition(lines);

        }

        /// <summary>
        /// Parses game board lines and returns new instance of GameBoardDefinition POCO class 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private GameBoardDefinition GetGameBoardDefinition(string[] lines)
        {
            // Game Board has previously been normalized so no file validation is needed
            // We are ensured that file is valid, lines contain valid info
            // and player start/end positions exists

            Point playerStartPos = new Point();
            Point playerEndPos = new Point();

            int boardWidth = lines[0].Length;
            int boardHeight = lines.Length;

            char[,] gameBoard = new char[boardWidth, boardHeight];
            bool[,] solution = new bool[boardWidth, boardHeight];   // Defaults everything to false

            // Fill game board and find player start/end positions
            for (var y = 0; y < boardHeight; y++)
            {
                string ln = lines[y];

                for (var x = 0; x < ln.Length; x++)
                {
                    char c = ln[x];

                    if (c == playerStartPositionChar)
                    {
                        playerStartPos = new Point(x, y);
                    }
                    else if (c == playerEndPositionChar)
                    {
                        playerEndPos = new Point(x, y);
                    }

                    gameBoard[x, y] = c;

                }
            }

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
        /// Validates game board for allowed characters and player start/end positions
        /// </summary>
        /// <param name="lines">Reference to a string array that defines game board</param>
        private void ValidateGameBoard(string[] lines)
        {
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

            // 1. Validate characters in string array
            // 2. Ensure player start/end positions are defined

            bool playerHasStartPosition = false;
            bool playerHasEndPosition = false;

            foreach (var line in lines)
            {
                EnsureLineHasValidCharacters(line);

                foreach (var ch in line)
                {
                    if (ch == playerStartPositionChar)
                    {
                        if (playerHasStartPosition)
                        {
                            throw new ArgumentException(Constants.ErrorPlayerStartPosDuplicate, nameof(lines));
                        }

                        playerHasStartPosition = true;
                    }
                    else if (ch == playerEndPositionChar)
                    {
                        if (playerHasEndPosition)
                        {
                            throw new ArgumentException(Constants.ErrorPlayerEndPosDuplicate, nameof(lines));
                        }

                        playerHasEndPosition = true;
                    }
                }
            }

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
        /// Extrudes lines in string array if their length is shorter than a value provided
        /// </summary>
        /// <param name="length">Needed length of lines</param>
        /// <param name="lines">Reference to a string array with lines to extrude</param>
        private void NormalizeBoardLines(int length, string[] lines)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                string ln = lines[i];

                int extrudeValue = length - ln.Length;

                if (extrudeValue > 0)
                {
                    ln += new string(EMPTY_CHAR, extrudeValue);
                    lines[i] = ln;
                }
            }
        }
    }

}
