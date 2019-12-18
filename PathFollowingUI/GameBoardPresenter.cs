using PathFollowingUI.Core;
using System;

namespace PathFollowingUI
{
    /// <summary>
    /// Represents a class that takes game board and displays its contents to a Console window
    /// </summary>
    public static class GameBoardPresenter
    {

        /// <summary>
        /// Displays game board to a Console window
        /// </summary>
        /// <param name="boardDefinition">Properties of a game board</param>
        /// <param name="currentWord">Current Word (part of a solution)</param>
        /// <param name="pathWord">Current Path</param>
        public static void DisplayBoard(GameBoardDefinition boardDefinition, string currentWord, string pathWord)
        {
            Console.Clear();

            for (var y = 0; y < boardDefinition.BoardHeight; y++)
            {
                for (var x = 0; x < boardDefinition.BoardWidth; x++)
                {
                    char c = boardDefinition.BoardData[x, y];

                    Console.ForegroundColor = ConsoleColor.Gray;

                    ConsoleColor cColor;

                    switch (boardDefinition.BoardSolution[x, y])
                    {
                        case false:
                        default:
                            cColor = ConsoleColor.Black;
                            break;

                        case true:
                            cColor = ConsoleColor.Yellow;
                            break;
                    }

                    //if (boardDefinition.BoardTunnels[x, y])
                    //    cColor = ConsoleColor.Red;

                    Console.BackgroundColor = cColor;
                    Console.Write(c);

                    if (x + 1 == boardDefinition.BoardWidth)
                        Console.Write(Environment.NewLine);
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine();
            Console.WriteLine($"WORD: {currentWord}");
            Console.WriteLine($"PATH: {pathWord}");

            System.Threading.Thread.Sleep(10);
        }

    }

}
