namespace PathFollowingUI.Core
{
    /// <summary>
    /// Represents a class with miscelaneous helper utils
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// Determines whether current X,Y position is withing 2D array bounds
        /// </summary>
        /// <param name="x">X position in 2D array</param>
        /// <param name="y">Y position in 2D array</param>
        /// <param name="width">Width of an array</param>
        /// <param name="height">Height of an array</param>
        public static bool AreValidBounds(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0)
                return false;

            if (x > width - 1 || y > height - 1)
                return false;

            return true;
        }

    }
}
