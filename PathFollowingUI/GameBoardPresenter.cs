using PathFollowingUI.Core;
using System;

namespace PathFollowingUI
{
    /// <summary>
    /// Provides functionality to display game board contents on the console.
    /// Used for visual presentation during path solving.
    /// </summary>
    public static class GameBoardPresenter
    {
        /// <summary>
        /// Displays the current state of the game board on the console.
        /// </summary>
        /// <param name="boardDefinition">Game board properties and state</param>
        /// <param name="currentWord">Current solution word being formed</param>
        /// <param name="pathWord">Current path being followed</param>
        public static void DisplayBoard(GameBoardDefinition boardDefinition, string currentWord, string pathWord)
        {
            // Clear the console before drawing the board
            Console.Clear();

            // Draw the board grid
            for (var y = 0; y < boardDefinition.BoardHeight; y++)
            {
                for (var x = 0; x < boardDefinition.BoardWidth; x++)
                {
                    char boardChar = boardDefinition.BoardData[x, y];
                    
                    // Set text color for board characters
                    Console.ForegroundColor = ConsoleColor.Gray;
                    
                    // Determine background color based on whether this position is part of the solution path
                    ConsoleColor backgroundColor = boardDefinition.BoardSolution[x, y] 
                        ? ConsoleColor.Yellow  // Part of solution path
                        : ConsoleColor.Black;  // Not part of solution path
                    
                    // Uncomment to highlight tunnels with a different color
                    // if (boardDefinition.BoardTunnels[x, y])
                    //    backgroundColor = ConsoleColor.Red;
                    
                    // Apply background color and write the character
                    Console.BackgroundColor = backgroundColor;
                    Console.Write(boardChar);
                    
                    // Add line break at the end of each row
                    if (x + 1 == boardDefinition.BoardWidth)
                        Console.Write(Environment.NewLine);
                }
            }

            // Reset console colors for status information
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            
            // Display current word and path information
            Console.WriteLine();
            Console.WriteLine($"WORD: {currentWord}");
            Console.WriteLine($"PATH: {pathWord}");
            
            // Brief pause to control animation speed
            System.Threading.Thread.Sleep(10);
        }
    }
}
