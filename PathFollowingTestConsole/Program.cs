using PathFollowingUI;
using System;
using System.IO;

namespace PathFollowingTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Path Following Test Console");
            
            // Test complex board
            TestBoard("../PathFollowingUI/Boards/complexBoard.txt", "ABCDEFG");

            Console.ReadLine();
        }
        
        static void TestBoard(string boardPath, string targetWord)
        {
            Console.WriteLine($"Testing board: {boardPath}");
            Console.WriteLine($"Target word: {targetWord}");
            
            try
            {
                var solver = new PathFollowingSolver(boardPath, targetWord);
                string solutionWord;
                string pathWord;
                
                Console.WriteLine("Attempting to solve...");
                var success = solver.Solve(out solutionWord, out pathWord);
                
                Console.WriteLine($"Solution found: {success}");
                if (success)
                {
                    Console.WriteLine($"Solution word: {solutionWord}");
                    Console.WriteLine($"Path: {pathWord}");
                }
                else
                {
                    Console.WriteLine("No solution found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner error: {ex.InnerException.Message}");
            }
        }
    }
}
