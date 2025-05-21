using PathFollowingUI.Core;
using System;
using System.Linq;

namespace PathFollowingUI
{
    /// <summary>
    /// Main program class for the Path Following Example application.
    /// Contains user interface and solver execution logic.
    /// </summary>
    class Program
    {
        #region Console UI Methods

        /// <summary>
        /// Writes a specified number of empty lines to the console.
        /// </summary>
        /// <param name="count">Number of empty lines to write</param>
        static void WriteEmptyLines(byte count)
        {
            for (var i = 0; i < count; i++)
                Console.WriteLine();
        }

        /// <summary>
        /// Displays the application header on the console.
        /// </summary>
        static void WriteHeader()
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("   >> Path Following Example <<");
            Console.WriteLine("   ============================");
            Console.WriteLine();
            Console.WriteLine("   By Zoran Bošnjak (zoran0406@hotmail.com)");

            WriteEmptyLines(3);
        }

        /// <summary>
        /// Prompts the user to choose a game board.
        /// </summary>
        static void WriteUserChooseBoard()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   Enter 1 for board1, 2 for board2 and 3 for board 3");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   >");
        }

        /// <summary>
        /// Prompts the user to decide whether to display path following animation.
        /// </summary>
        static void WriteUserChoosePresenter()
        {
            WriteEmptyLines(2);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   Display path following on screen (Y/N)?");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   >");
        }

        /// <summary>
        /// Displays the solution result to the user.
        /// </summary>
        /// <param name="isSolved">Whether a solution was found</param>
        /// <param name="solutionWord">The solution word found</param>
        /// <param name="pathWord">The path word representing the solution path</param>
        static void WritePathSolution(bool isSolved, string solutionWord, string pathWord)
        {
            WriteEmptyLines(2);

            if (isSolved)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   Solution Found!");
                Console.WriteLine();
                Console.WriteLine($"   FINAL WORD: {solutionWord}");
                Console.WriteLine($"   FINAL PATH: {pathWord}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Unfortunately solution could not be found.");
            }

            WriteEmptyLines(2);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Enter any key for a title screen >");
        }

        /// <summary>
        /// Displays a colorful goodbye message to the user.
        /// </summary>
        static void WriteGoodbye()
        {
            ConsoleColor odd = ConsoleColor.Magenta;
            ConsoleColor even = ConsoleColor.Cyan;

            WriteEmptyLines(5);

            string message = "#####   (-: Thank you and goodbye. :-)   #####";

            for (var i = 0; i < message.Length; i++)
            {
                ConsoleColor textColor = (i % 2 == 0) ? odd : even;

                Console.ForegroundColor = textColor;
                Console.Write(message[i]);
            }

            WriteEmptyLines(3);
        }
        #endregion

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            // Valid board options that the user can select
            char[] validBoard = { '1', '2', '3' };
            
            // Main application loop
            bool continueRunning = true;
            while (continueRunning)
            {
                // Get user's board selection
                char userInputBoard = GetBoardSelection(validBoard);
                
                // Get user's display preference
                bool userDisplayPresenter = GetDisplayPreference();
                
                // Wait briefly so user can see their selection
                System.Threading.Thread.Sleep(500);
                Console.Clear();
                
                // Run the path solver with selected options
                RunSolver(userInputBoard, userDisplayPresenter);
                
                // After seeing results, we return to the main menu automatically
            }
        }
        
        /// <summary>
        /// Gets the user's board selection.
        /// </summary>
        /// <param name="validBoards">Array of valid board selection characters</param>
        /// <returns>The selected board character</returns>
        private static char GetBoardSelection(char[] validBoards)
        {
            while (true)
            {
                Console.Clear();
                WriteHeader();
                WriteUserChooseBoard();

                var keyInfo = Console.ReadKey();
                char selection = keyInfo.KeyChar;

                if (validBoards.Contains(selection))
                    return selection;
            }
        }
        
        /// <summary>
        /// Gets the user's preference for displaying the path following animation.
        /// </summary>
        /// <returns>True if user wants to see the animation, false otherwise</returns>
        private static bool GetDisplayPreference()
        {
            WriteUserChoosePresenter();
            var keyInfo = Console.ReadKey(true);
            bool preference = (keyInfo.KeyChar == 'Y' || keyInfo.KeyChar == 'y');
            
            Console.Write(preference ? "Y" : "N");
            return preference;
        }

        /// <summary>
        /// Runs the path solver with the selected board and display preferences.
        /// </summary>
        /// <param name="userInputBoard">The selected board character</param>
        /// <param name="userDisplayPresenter">Whether to display path following animation</param>
        private static void RunSolver(char userInputBoard, bool userDisplayPresenter)
        {
            string boardName;
            string boardSolution;
            string boardPath = "Boards/";

            // Determine board file and solution based on user selection
            switch (userInputBoard)
            {
                case '1':
                default:
                    boardName = boardPath + "board1.txt";
                    boardSolution = "ACB";
                    break;

                case '2':
                    boardName = boardPath + "board2.txt";
                    boardSolution = "ABCD";
                    break;

                case '3':
                    boardName = boardPath + "board3.txt";
                    boardSolution = "BEEFCAKE";
                    break;
            }

            // Create solver instance
            var solver = new PathFollowingSolver(boardName, boardSolution);

            // Register for position change events if animation is requested
            if (userDisplayPresenter)
            {
                solver.PlayerPositionChanged += Solver_PlayerPositionChanged;
            }

            // Solve the path and display results
            bool isSolved = solver.Solve(out string solutionWord, out string pathWord);
            WritePathSolution(isSolved, solutionWord, pathWord);

            // Wait for user acknowledgment
            Console.ReadKey();
            WriteGoodbye();
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Event handler for player position changes during path solving.
        /// Displays the current board state with the solution progress.
        /// </summary>
        /// <param name="gameBoard">Current game board definition</param>
        /// <param name="currentWord">Current solution word</param>
        /// <param name="pathWord">Current path word</param>
        private static void Solver_PlayerPositionChanged(GameBoardDefinition gameBoard, string currentWord, string pathWord)
        {
            // Display game board with the current solution to the Console window
            GameBoardPresenter.DisplayBoard(gameBoard, currentWord, pathWord);
        }
    }
}
