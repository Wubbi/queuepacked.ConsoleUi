using System;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Stores the visuals to show in the console
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// The character used to mark a Symbol as transparent
        /// </summary>
        public const char Invisible = char.MinValue;

        private readonly int _top;
        private readonly int _left;

        private readonly int _width;
        private readonly int _height;

        private readonly Symbol[,] _screenCurrent;
        private readonly Symbol[,] _screenLastDrawn;

        private readonly object _lock;

        internal Buffer(int top, int width, int height)
        {
            if (top < 0)
                throw new ArgumentOutOfRangeException(nameof(top));

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;
            _left = 0;
            _top = top;

            _screenCurrent = new Symbol[width, height];
            _screenLastDrawn = new Symbol[width, height];

            _lock = new object();
        }

        internal void Draw(bool fullRedraw)
        {
            lock (_lock)
            {
                for (int y = 0; y < _height; ++y)
                {
                    for (int x = 0; x < _width; ++x)
                    {
                        Symbol symbol = _screenCurrent[x, y];
                        if (!fullRedraw && symbol.Equals(in _screenLastDrawn[x, y]))
                            continue;

                        _screenLastDrawn[x, y] = symbol;

                        Console.BackgroundColor = symbol.BackgroundColor;
                        Console.ForegroundColor = symbol.ForegroundColor;
                        Console.SetCursorPosition(_left + x, _top + y);
                        Console.Write(symbol.Character);
                    }
                }

                Console.SetCursorPosition(_left, _top);
            }
        }

        /// <summary>
        /// Draws a rectangle to this <see cref="Buffer"/> using the given <see cref="Symbol"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="symbol"></param>
        public void Rect(int x, int y, int width, int height, Symbol symbol)
        {
            if (symbol.Character == Invisible)
                return;

            int xLimit = Math.Min(x + width, _width);
            int yLimit = Math.Min(y + height, _height);

            lock (_lock)
            {
                for (int i = Math.Max(y, 0); i < yLimit; ++i)
                    for (int j = Math.Max(x, 0); j < xLimit; ++j)
                        _screenCurrent[j, i] = symbol;
            }
        }

        /// <summary>
        /// Draws a <see cref="Pattern"/> to this <see cref="Buffer"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pattern"></param>
        public void Pattern(int x, int y, Pattern pattern)
        {
            if (pattern is null)
                throw new ArgumentNullException(nameof(pattern));

            int xLimit = Math.Min(x + pattern.Width, _width);
            int yLimit = Math.Min(y + pattern.Height, _height);

            int k = 0;
            int l;
            lock (_lock)
            {
                for (int i = Math.Max(y, 0); i < yLimit; ++i, ++k)
                {
                    l = 0;
                    for (int j = Math.Max(x, 0); j < xLimit; ++j, ++l)
                    {
                        if (pattern[l, k].Character != Invisible)
                            _screenCurrent[j, i] = pattern[l, k];
                    }
                }
            }
        }

        /// <summary>
        /// Draws a sequence of Symbols to this <see cref="Buffer"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="symbols"></param>
        public void Symbols(int x, int y, params Symbol[] symbols)
        {
            if (symbols.Length < 1)
                return;

            if (y < 0 || y >= _height)
                return;

            int xLimit = Math.Min(x + symbols.Length, _width);

            int i = 0;

            lock (_lock)
            {
                for (int j = Math.Max(x, 0); j < xLimit; ++j, ++i)
                    if (symbols[i].Character != Invisible)
                        _screenCurrent[j, y] = symbols[i];
            }
        }

        /// <summary>
        /// Fills the entire Buffer with the given Symbol
        /// </summary>
        /// <param name="filler"></param>
        public void Clear(Symbol filler)
        {
            lock (_lock)
            {
                for (int y = 0; y < _height; ++y)
                    for (int x = 0; x < _width; ++x)
                        _screenCurrent[x, y] = filler;
            }
        }
    }
}
