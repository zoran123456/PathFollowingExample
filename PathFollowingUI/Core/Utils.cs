namespace PathFollowingUI.Core
{
    /// <summary>
    /// Provides miscellaneous helper utility methods used throughout the application.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Determines whether the specified position is within the bounds of a 2D array.
        /// </summary>
        /// <param name="x">X position in the 2D array</param>
        /// <param name="y">Y position in the 2D array</param>
        /// <param name="width">Width of the array</param>
        /// <param name="height">Height of the array</param>
        /// <returns>True if the position is within bounds, false otherwise</returns>
        public static bool AreValidBounds(int x, int y, int width, int height)
        {
            // Check for negative coordinates
            if (x < 0 || y < 0)
                return false;

            // Check if coordinates exceed array dimensions
            if (x > width - 1 || y > height - 1)
                return false;

            return true;
        }
    }
}
