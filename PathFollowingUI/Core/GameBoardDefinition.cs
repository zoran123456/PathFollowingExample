using System.Drawing;

namespace PathFollowingUI.Core
{

    /// <summary>
    /// Defines simple POCO object used to store game board information
    /// </summary>
    public class GameBoardDefinition
    {
        /// <summary>
        /// Definition of a actual game board
        /// </summary>
        public char[,] BoardData { get; set; }

        /// <summary>
        /// Definitions of game board solution
        /// </summary>
        public bool[,] BoardSolution { get; set; }

        /// <summary>
        /// Definitions of game board "tunnels" (there is possibility of solution to cross existing solution)
        /// </summary>
        public bool[,] BoardTunnels { get; set; }

        /// <summary>
        /// Defines word solution for a game board
        /// </summary>
        public string BoardWordSolution { get; set; }

        /// <summary>
        /// Defines player starting position
        /// </summary>
        public Point PlayerStartPosition { get; set; }

        /// <summary>
        /// Defines player end position
        /// </summary>
        public Point PlayerEndPosition { get; set; }

        /// <summary>
        /// Raw file lines (normalized length)
        /// </summary>
        public string[] RawFile { get; set; }

        /// <summary>
        /// Defines Width of a game board
        /// </summary>
        public int BoardWidth { get; set; }

        /// <summary>
        /// Defines Height of a game board
        /// </summary>
        public int BoardHeight { get; set; }
    }


}
