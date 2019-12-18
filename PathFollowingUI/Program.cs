using PathFollowingUI.Core;
using System;
using System.Linq;

namespace PathFollowingUI
{
    class Program
    {

        #region Just a simple string templates that are shown on a screen

        static void WriteEmptyLines(byte count)
        {
            for (var i = 0; i < count; i++)
                Console.WriteLine();
        }

        static void WriteHeader()
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("   >> Path Following Example <<");
            Console.WriteLine("   ============================");
            Console.WriteLine();
            Console.WriteLine("   By Zoran Bošnjak (zoran0406@hotmail.com)");

            WriteEmptyLines(3);

        }

        static void WriteUserChooseBoard()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   Enter 1 for board1, 2 for board2 and 3 for board 3");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   >");
        }

        static void WriteUserChoosePresenter()
        {
            WriteEmptyLines(2);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   Display path following on screen (Y/N)?");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   >");
        }

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

        static void Main(string[] args)
        {
            char userInputBoard;        // what board we need (1,2 or 3)
            bool userDisplayPresenter;  // shall we display path following on a screen (as a flickery, ugly animation)?

            char[] validBoard = { '1', '2', '3' };

        begining:
            // Get board name from user input

            while (true)
            {
                Console.Clear();

                WriteHeader();

                WriteUserChooseBoard();

                var q1Key = Console.ReadKey();
                userInputBoard = q1Key.KeyChar;

                if (validBoard.Contains(userInputBoard))
                    break;
            }


            // Check if user wants to display how path following works on a screen

            WriteUserChoosePresenter();
            var q2Key = Console.ReadKey(true);
            userDisplayPresenter = (q2Key.KeyChar == 'Y' || q2Key.KeyChar == 'y');

            Console.Write((userDisplayPresenter) ? "Y" : "N");


            // Wait for half of a second for a user to see what has he selected
            // After that clear screen and begin solving

            System.Threading.Thread.Sleep(500);
            Console.Clear();

            RunSolver(userInputBoard, userDisplayPresenter);

            goto begining;
        }


        private static void RunSolver(char userInputBoard, bool userDisplayPresenter)
        {
            string boardName;
            string boardSolution;

            string boardPath = "Boards/";

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

            // Create solver (it calls backtracking solver which is hardcoded), class can be extended 
            // to support dependency injection if needed
            var solver = new PathFollowingSolver(boardName, boardSolution);

            // If user has decided to see how path following works
            // create a event that will trigger every time path position changes
            // If event isn't created, no trigger will happen
            if (userDisplayPresenter)
            {

                solver.PlayerPositionChanged += Solver_PlayerPositionChanged;

            }



            bool isSolved = solver.Solve(out string solutionWord, out string pathWord);

            WritePathSolution(isSolved, solutionWord, pathWord);

            Console.ReadKey();

            WriteGoodbye();

            System.Threading.Thread.Sleep(1000);
        }

        private static void Solver_PlayerPositionChanged(GameBoardDefinition gameBoard, string currentWord, string pathWord)
        {

            // Display game board with a current solution to a Console window

            GameBoardPresenter.DisplayBoard(gameBoard, currentWord, pathWord);

        }

    }
}
