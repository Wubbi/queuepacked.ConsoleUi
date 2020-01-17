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
            BackgroundColor = Console.BackgroundColor;
            ForegroundColor = Console.ForegroundColor;
            CursorLeft = Console.CursorLeft;
            CursorTop = Console.CursorTop;
            TreatControlCAsInput = Console.TreatControlCAsInput;
            Title = "";

            if (!UiHub.IsWindows)
                return;

            WindowWidth = Console.WindowWidth;
            WindowHeight = Console.WindowHeight;
            Title = Console.Title;
            CursorSize = Console.CursorSize;
            CursorVisible = Console.CursorVisible;
        }

        /// <summary>
        /// Writes the values of this instance to <see cref="Console"/>
        /// </summary>
        internal void Set()
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.SetCursorPosition(CursorLeft, CursorTop);
            Console.TreatControlCAsInput = TreatControlCAsInput;

            if (!UiHub.IsWindows)
                return;

            Console.Title = Title;
            Console.CursorVisible = CursorVisible;
            Console.CursorSize = CursorSize;
            Console.WindowWidth = WindowWidth;
            Console.WindowHeight = WindowHeight;
        }
    }
}
