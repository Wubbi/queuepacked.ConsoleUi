using System;

namespace queuepacked.ConsoleUI
{
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
}