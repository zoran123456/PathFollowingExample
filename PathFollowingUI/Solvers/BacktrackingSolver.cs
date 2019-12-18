using PathFollowingUI.Core;
using System.Drawing;
using System.Linq;


namespace PathFollowingUI.Solvers
{
    /// <summary>
    /// Represents class that provides means for solving board using backtracking algorithm
    /// </summary>
    public class BacktrackingSolver
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


        private readonly GameBoardDefinition boardDefinition;

        /// <summary>
        /// Definition of a actual game board
        /// </summary>
        private char[,] GameBoard
        {
            get
            {
                return boardDefinition.BoardData;
            }
        }

        /// <summary>
        /// Definitions of game board solution
        /// </summary>
        private bool[,] Solution
        {
            get
            {
                return boardDefinition.BoardSolution;
            }
        }

        /// <summary>
        /// Definitions of game board "tunnels" (there is possibility of solution to cross existing solution)
        /// </summary>
        private bool[,] Tunnels
        {
            get
            {
                return boardDefinition.BoardTunnels;
            }
        }

        /// <summary>
        /// Defines player end position
        /// </summary>
        private Point PlayerEndPos
        {
            get
            {
                return boardDefinition.PlayerEndPosition;
            }
        }

        /// <summary>
        /// Defines needed word for a board solution
        /// </summary>
        private string BoardWord { get; set; }

        /// <summary>
        /// Initializes new instance of Backtracking solver class and populates properties taken from boardDefinition
        /// </summary>
        /// <param name="boardDefinition">Properties of a game board</param>
        public BacktrackingSolver(GameBoardDefinition boardDefinition)
        {
            this.boardDefinition = boardDefinition;
        }

        /// <summary>
        /// Determines whether current X,Y position is withing game board bounds
        /// </summary>
        bool AreValidBounds(int x, int y)
        {
            return Utils.AreValidBounds(x, y, boardDefinition.BoardWidth, boardDefinition.BoardHeight);
        }

        /// <summary>
        /// Checks whether X,Y player position (possible future position, i.e. next move) are valid. It checks for bounds, character beneath, tunnel and solution word
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="currentWord"></param>
        /// <returns></returns>
        bool IsValidPosition(int x, int y, string currentWord)
        {
            // X,Y is out of bounds of a game board
            if (!AreValidBounds(x, y))
                return false;

            // Possible position is not under valid board character (i.e. collides with a wall, or empty space)
            if (!VALID_BOARD_CHARS.Contains(GameBoard[x, y]))
                return false;

            // Possible position collides with previously visited place
            // This is *SUPER* important, without this check Stack Overflow error occurs
            // (as there is possibility of endless loop)
            if (Solution[x, y] && !Tunnels[x, y])
                return false;

            // Final step is to check whether placing player on a position will cause solution word to be invalid
            if (!BoardWord.StartsWith(currentWord))
                return false;

            return true;
        }

        private string solutionWord;
        private string solutionPathWord;

        /// <summary>
        /// Backtracking and Recursion method, places player position on a X,Y and calls itself again until solution is found
        /// </summary>
        /// <param name="x">Player X position</param>
        /// <param name="y">Player Y position</param>
        /// <param name="prevMovement"></param>
        /// <param name="currentWord">Current Word (part of a solution) that will be "collected" after visiting places</param>
        /// <param name="pathWord">Current Path that will be "collected" after visiting places</param>
        /// <returns></returns>
        bool MoveNextStep(int x, int y, char prevMovement, string currentWord, string pathWord)
        {
            
            PlayerPositionChanged?.Invoke(boardDefinition, currentWord, pathWord);


            string fltWord = new string(currentWord.Where(c => ALPHABET.Contains(c)).ToArray());


            if (fltWord == BoardWord)
            {
                if (x == PlayerEndPos.X && y == PlayerEndPos.Y)
                {
                    Solution[x, y] = true;
                    pathWord += GameBoard[x, y];

                    
                    solutionWord = currentWord;
                    solutionPathWord = pathWord;

                    return true;
                }
            }

            char c = ' ';

            if (AreValidBounds(x, y))
            {
                c = GameBoard[x, y];
                if (ALPHABET.Contains(c))
                    c = '+';

                if (Solution[x, y] && Tunnels[x, y])
                    c = prevMovement;
            }


            if (IsValidPosition(x, y, fltWord))
            {
                if (Solution[x, y])
                    c = prevMovement;


                pathWord += GameBoard[x, y];

              
                if (ALPHABET.Contains(GameBoard[x, y]) && !Solution[x, y])
                    currentWord += GameBoard[x, y];

                Solution[x, y] = true;


                switch (c)
                {
                    case (char)BoardLetterDefinition.MoveHorizontal:
                        // Move horizontally


                        if (MoveNextStep(x - 1, y, c, currentWord, pathWord))
                            return true;
                        if (MoveNextStep(x + 1, y, c, currentWord, pathWord))
                            return true;
                        break;

                    case (char)BoardLetterDefinition.MoveVertical:
                        // Move vertically


                        if (MoveNextStep(x, y - 1, c, currentWord, pathWord))
                            return true;
                        if (MoveNextStep(x, y + 1, c, currentWord, pathWord))
                            return true;
                        break;

                    case (char)BoardLetterDefinition.MoveAnywhere:
                    case (char)BoardLetterDefinition.PlayerStartPosition:
                        // Move in each direction


                        if (MoveNextStep(x + 1, y, c, currentWord, pathWord)) return true;
                        if (MoveNextStep(x, y + 1, c, currentWord, pathWord)) return true;
                        if (MoveNextStep(x - 1, y, c, currentWord, pathWord)) return true;
                        if (MoveNextStep(x, y - 1, c, currentWord, pathWord)) return true;
                        break;
                }

                if (!Tunnels[x, y])
                    Solution[x, y] = false;

                // Simplified, the same as currentWord.Substring(0, currentWord.Length -1)
                currentWord = currentWord[0..^1];
                pathWord = pathWord[0..^1];

                return false;
            }

            return false;
        }


        public (bool SolutionFound, string SolutionWord, string PathWord) FindSolution
            (int x, int y, string boardWordSolution, bool[,] solution)
        {
            solution = Solution;

            BoardWord = boardWordSolution;


            bool found = MoveNextStep(x, y, ' ', "", "");



            return (SolutionFound: found, SolutionWord: solutionWord, PathWord: solutionPathWord);

        }

    }

}
