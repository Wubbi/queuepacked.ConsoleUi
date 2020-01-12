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

    /// <summary>
    /// A collection of Symbols arranged in a rectangle
    /// </summary>
    public class Pattern
    {
        public static readonly Pattern Empty;

        static Pattern()
        {
            Empty = new Pattern(0, 0);
        }

        private readonly Symbol[,] _symbols;

        /// <summary>
        /// The Width of this pattern
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The Height of this pattern
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Returns the <see cref="Symbol"/> at the given coordinates in this <see cref="Pattern"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Symbol this[int x, int y] => _symbols[x, y];

        private Pattern(int width, int height)
        {
            Width = width;
            Height = height;

            _symbols = new Symbol[Width, Height];
        }

        private Pattern(Pattern source)
        {
            Width = source.Width;
            Height = source.Height;

            _symbols = new Symbol[Width, Height];

            Array.Copy(source._symbols, _symbols, _symbols.Length);
        }

        /// <summary>
        /// Creates a depp copy of this <see cref="Pattern"/>
        /// </summary>
        /// <returns></returns>
        public Pattern Clone()
        {
            return new Pattern(this);
        }

        /// <summary>
        /// A helper class to create instances of <see cref="Pattern"/>
        /// </summary>
        public class Builder
        {
            private readonly Pattern _pattern;

            private Builder(int width, int height)
            {
                if (width < 1)
                    throw new ArgumentOutOfRangeException(nameof(width));

                if (height < 1)
                    throw new ArgumentOutOfRangeException(nameof(height));

                _pattern = new Pattern(width, height);
            }

            /// <summary>
            /// Creates a new Builder
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public static Builder New(int width, int height)
                => new Builder(width, height);

            /// <summary>
            /// Write one or more Symbols in a row from left to right to this patterns starting at the given coordinates
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="symbols"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public Builder AddSymbols(int x, int y, params Symbol[] symbols)
            {
                if (x < 0 || x >= _pattern.Width)
                    throw new ArgumentOutOfRangeException(nameof(x));

                if (y < 0 || y >= _pattern.Height)
                    throw new ArgumentOutOfRangeException(nameof(y));

                if (symbols.Length < 1)
                    return this;

                if (x + symbols.Length > _pattern.Width)
                    throw new ArgumentOutOfRangeException(nameof(symbols), "Too many symbols");

                for (int i = 0; i < symbols.Length; ++i)
                    _pattern._symbols[x + i, y] = symbols[i];

                return this;
            }

            /// <summary>
            /// Write a string with the given colors into this pattern
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="text"></param>
            /// <param name="backgroundColor"></param>
            /// <param name="foregroundColor"></param>
            /// <returns></returns>
            public Builder AddText(int x, int y, string text, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
            {
                if (x < 0 || x >= _pattern.Width)
                    throw new ArgumentOutOfRangeException(nameof(x));

                if (y < 0 || y >= _pattern.Height)
                    throw new ArgumentOutOfRangeException(nameof(y));

                if (text is null)
                    throw new ArgumentNullException(nameof(text));

                if (text.Length < 1)
                    return this;

                if (x + text.Length > _pattern.Width)
                    throw new ArgumentOutOfRangeException(nameof(text), "Text too long");

                for (int i = 0; i < text.Length; ++i)
                    _pattern._symbols[x + i, y] = new Symbol(text[i], backgroundColor, foregroundColor);

                return this;
            }

            /// <summary>
            /// Draws a full rect
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="filler"></param>
            /// <returns></returns>
            public Builder Rect(int x, int y, int width, int height, Symbol filler)
            {
                if (x < 0 || x >= _pattern.Width)
                    throw new ArgumentOutOfRangeException(nameof(x));

                if (y < 0 || y >= _pattern.Height)
                    throw new ArgumentOutOfRangeException(nameof(y));

                if (width < 1 || width + x > _pattern.Width)
                    throw new ArgumentOutOfRangeException(nameof(width));

                if (height < 1 || height + y > _pattern.Height)
                    throw new ArgumentOutOfRangeException(nameof(height));

                for (int j = 0; j < height; ++j)
                    for (int i = 0; i < width; ++i)
                        _pattern._symbols[x + i, y + j] = filler;

                return this;
            }

            /// <summary>
            /// Creates a new <see cref="Pattern"/> using the current state of this <see cref="Builder"/>
            /// </summary>
            /// <returns></returns>
            public Pattern Create()
            {
                return new Pattern(_pattern);
            }
        }
    }

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
