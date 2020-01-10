using System;

namespace queuepacked.ConsoleUI
{
    internal class ConsoleSettings
    {
        internal int CursorSize { get; }

        internal ConsoleColor BackgroundColor { get; }

        internal ConsoleColor ForegroundColor { get; }

        internal bool CursorVisible { get; }

        internal int CursorLeft { get; }

        internal int CursorTop { get; }

        internal string Title { get; }

        internal bool TreatControlCAsInput { get; }

        internal int WindowWidth { get; }

        internal int WindowHeight { get; }

        /// <summary>
        /// Create a new instance using the currently used settings in <see cref="Console"/>
        /// </summary>
        internal ConsoleSettings()
        {
            CursorSize = Console.CursorSize;
            BackgroundColor = Console.BackgroundColor;
            ForegroundColor = Console.ForegroundColor;
            CursorVisible = Console.CursorVisible;
            CursorLeft = Console.CursorLeft;
            CursorTop = Console.CursorTop;
            Title = Console.Title;
            TreatControlCAsInput = Console.TreatControlCAsInput;
            WindowWidth = Console.WindowWidth;
            WindowHeight = Console.WindowHeight;
        }

        /// <summary>
        /// Writes the values of this instance to <see cref="Console"/>
        /// </summary>
        internal void Set()
        {
            Console.CursorSize = CursorSize;
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.CursorVisible = CursorVisible;
            Console.CursorLeft = CursorLeft;
            Console.CursorTop = CursorTop;
            Console.Title = Title;
            Console.TreatControlCAsInput = TreatControlCAsInput;
            Console.WindowWidth = WindowWidth;
            Console.WindowHeight = WindowHeight;
        }
    }
}
