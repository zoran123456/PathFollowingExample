using System;
using System.Linq;

namespace PathFollowingUI
{
    class Program
    {
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


        static void Main(string[] args)
        {
            char userInputBoard;
            bool userDisplayPresenter;

            char[] validBoard = { '1', '2', '3' };

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


            WriteUserChoosePresenter();
            var q2Key = Console.ReadKey(true);
            userDisplayPresenter = (q2Key.KeyChar == 'Y' || q2Key.KeyChar == 'y');

            Console.Write((userDisplayPresenter) ? "Y" : "N");


        }

    }
}
