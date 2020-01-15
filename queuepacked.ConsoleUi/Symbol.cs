using System;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// A single character to display in the console
    /// </summary>
    public struct Symbol
    {
        /// <summary>
        /// The character to display
        /// </summary>
        public readonly char Character;

        /// <summary>
        /// The background color to use for this Symbol
        /// </summary>
        public readonly ConsoleColor BackgroundColor;

        /// <summary>
        /// The foreground color to use for this Symbol
        /// </summary>
        public readonly ConsoleColor ForegroundColor;

        /// <summary>
        /// Creates a new Symbol using the given values
        /// </summary>
        /// <param name="character"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="foregroundColor"></param>
        public Symbol(char character, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            Character = character;
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// Compares this <see cref="Symbol"/> to another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(in Symbol other)
        {
            if (Character != other.Character)
                return false;

            if (BackgroundColor != other.BackgroundColor)
                return false;

            return ForegroundColor == other.ForegroundColor;
        }
    }
}