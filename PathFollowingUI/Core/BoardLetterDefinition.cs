namespace PathFollowingUI.Core
{
    /// <summary>
    /// Defines special characters used in the game board to control movement and positions.
    /// </summary>
    public enum BoardLetterDefinition
    {
        /// <summary>
        /// Represents the player's starting position on the board.
        /// </summary>
        PlayerStartPosition = '@',

        /// <summary>
        /// Represents the player's ending position (destination) on the board.
        /// </summary>
        PlayerEndPosition = 'x',

        /// <summary>
        /// Indicates that the player can move in all four directions (up, down, left, right).
        /// </summary>
        MoveAnywhere = '+',

        /// <summary>
        /// Indicates that the player can only move horizontally (left or right).
        /// </summary>
        MoveHorizontal = '-',

        /// <summary>
        /// Indicates that the player can only move vertically (up or down).
        /// </summary>
        MoveVertical = '|'
    }
}
