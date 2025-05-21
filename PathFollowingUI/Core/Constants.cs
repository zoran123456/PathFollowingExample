namespace PathFollowingUI.Core
{
    /// <summary>
    /// Provides constants used throughout the application, primarily for error messages.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Error message for when a file is not found or the path is invalid.
        /// </summary>
        public const string ErrorFileNotFound = "Must point to a valid and existing filename.";

        /// <summary>
        /// Error message for when a file is invalid, empty, or has incorrect encoding.
        /// </summary>
        public const string ErrorFileInvalid = "File is invalid, empty, or wrong file encoding.";

        /// <summary>
        /// Error message for when a required value is empty.
        /// </summary>
        public const string ErrorValueIsEmpty = "Value cannot be empty.";

        /// <summary>
        /// Error message template for when a line contains invalid characters.
        /// {0} will be replaced with the invalid character.
        /// </summary>
        public const string ErrorLineHasInvalidCharacters = "Line contains invalid character(s). Invalid character: \"{0}\"";

        /// <summary>
        /// Error message for when multiple player start positions are defined.
        /// </summary>
        public const string ErrorPlayerStartPosDuplicate = "Player starting position is defined two or more times.";

        /// <summary>
        /// Error message for when multiple player end positions are defined.
        /// </summary>
        public const string ErrorPlayerEndPosDuplicate = "Player end position is defined two or more times.";

        /// <summary>
        /// Error message for when player start position is not defined.
        /// </summary>
        public const string ErrorPlayerStartPosNotFound = "Player starting position is not defined.";

        /// <summary>
        /// Error message for when player end position is not defined.
        /// </summary>
        public const string ErrorPlayerEndPosNotFound = "Player end position is not defined.";
    }
}
