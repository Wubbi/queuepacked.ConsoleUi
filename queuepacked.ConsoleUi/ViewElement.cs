using System;
using System.Runtime.CompilerServices;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Base of all view objects
    /// </summary>
    public abstract class ViewElement : IElement
    {
        private int _x;
        private int _y;

        private int _width;
        private int _height;

        private ConsoleColor _backgroundColor;
        private ConsoleColor _foregroundColor;

        private bool _visible;

        /// <inheritdoc cref="IElement.X"/>
        public int X
        {
            get => _x;
            set
            {
                if (_x == value)
                    return;

                _x = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="IElement.Y"/>
        public int Y
        {
            get => _y;
            set
            {
                if (_y == value)
                    return;

                _y = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="IElement.Width"/>
        public int Width
        {
            get => _width;
            set
            {
                if (_width == value)
                    return;

                _width = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="IElement.Height"/>
        public int Height
        {
            get => _height;
            set
            {
                if (_height == value)
                    return;

                _height = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets this elements Background color
        /// </summary>
        public ConsoleColor BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value)
                    return;

                _backgroundColor = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets this elements Foreground color
        /// </summary>
        public ConsoleColor ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                if (_foregroundColor == value)
                    return;

                _foregroundColor = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Whether or not to show this element
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Whether or not this element had changes that require it to be redrawn
        /// </summary>
        internal bool Dirty { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ViewElement"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private protected ViewElement(int x, int y, int width, int height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _x = x;
            _y = y;
            _width = width;
            _height = height;

            _backgroundColor = ConsoleColor.Black;
            _foregroundColor = ConsoleColor.Gray;

            _visible = true;

            Dirty = true;
        }

        /// <summary>
        /// Marks this element as needing a redraw
        /// </summary>
        private protected void PropertyChanged([CallerMemberName] string propertyName = "")
        {
            Dirty = true;
            OnPropertyChanged(propertyName);
            RedrawRequired?.Invoke(this);
        }

        internal void Draw(Buffer buffer)
        {
            Dirty = false;
            OnDraw(buffer);
        }

        /// <summary>
        /// Called when this element is drawn on it's Views Buffer
        /// </summary>
        /// <param name="buffer"></param>
        protected abstract void OnDraw(Buffer buffer);

        /// <summary>
        /// Called whenever this element is changed
        /// </summary>
        /// <param name="callerMember">The property that was changed</param>
        protected virtual void OnPropertyChanged(string callerMember) { }

        internal event Action<ViewElement>? RedrawRequired;
    }
}
