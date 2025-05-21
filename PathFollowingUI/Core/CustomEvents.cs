namespace PathFollowingUI.Core
{
    /// <summary>
    /// Contains custom event definitions used throughout the application.
    /// </summary>
    public class CustomEvents
    {
        /// <summary>
        /// Delegate for handling player position change events during path solving.
        /// </summary>
        /// <param name="gameBoard">Current game board definition</param>
        /// <param name="currentWord">Current solution word</param>
        /// <param name="pathWord">Current path word</param>
        public delegate void PlayerPositionChangedHandler(GameBoardDefinition gameBoard, string currentWord, string pathWord);
    }
}
