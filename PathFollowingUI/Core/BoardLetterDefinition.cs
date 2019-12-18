namespace PathFollowingUI.Core
{
    /// <summary>
    /// Definitions of a letters in a board
    /// </summary>
    public enum BoardLetterDefinition
    {
        /// <summary>
        /// Player Starting Position
        /// </summary>
        PlayerStartPosition = '@',

        /// <summary>
        /// Player Ending Position (game solution)
        /// </summary>
        PlayerEndPosition = 'x',

        /// <summary>
        /// Player can move in each of 4 directions
        /// </summary>
        MoveAnywhere = '+',

        /// <summary>
        /// Player can move only horizontally (left or right from current position)
        /// </summary>
        MoveHorizontal = '-',

        /// <summary>
        /// Player can move only vertically (up or down from current position)
        /// </summary>
        MoveVertical = '|'
    }

}
