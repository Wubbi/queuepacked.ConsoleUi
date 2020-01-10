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

    /// <summary>
    /// A type of <see cref="ViewElement"/> which can be interacted with
    /// </summary>
    public abstract class InteractiveViewElement : ViewElement
    {
        private bool _enabled;

        private bool _selected;

        private ConsoleColor _backgroundColorSelected;

        private ConsoleColor _foregroundColorSelected;

        /// <summary>
        /// Whether or not this element can be selected with
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Whether or not this element is currently selected
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;

                _selected = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets this elements Background color
        /// </summary>
        public ConsoleColor BackgroundColorSelected
        {
            get => _backgroundColorSelected;
            set
            {
                if (_backgroundColorSelected == value)
                    return;

                _backgroundColorSelected = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets this elements Foreground color
        /// </summary>
        public ConsoleColor ForegroundColorSelected
        {
            get => _foregroundColorSelected;
            set
            {
                if (_foregroundColorSelected == value)
                    return;

                _foregroundColorSelected = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// A number used in relation to other elements to determine the tab order
        /// </summary>
        public int Tabindex { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="InteractiveViewElement"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected InteractiveViewElement(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _enabled = true;
            _selected = false;
            _backgroundColorSelected = ConsoleColor.Gray;
            _foregroundColorSelected = ConsoleColor.Black;
            Tabindex = 0;
        }

        internal virtual void NewInput(InputEventArgs e)
        {
        }
    }

    /// <summary>
    /// Marks an element as having Text content to display
    /// </summary>
    public interface ITextElement
    {
        /// <summary>
        /// Gets or sets the Text displayed in this element
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the horizontal arrangement of text in this element
        /// </summary>
        AlignmentHorizontal TextAlignmentHorizontal { get; set; }

        /// <summary>
        /// Gets or sets the vertical arrangement of text in this element
        /// </summary>
        AlignmentVertical TextAlignmentVertical { get; set; }

        /// <summary>
        /// Gets or sets the Filler used to pad the text
        /// </summary>
        char Filler { get; set; }

        /// <summary>
        /// Whether or not text should continue in a new line if the element isn't wide enough
        /// </summary>
        bool WrapText { get; set; }
    }

    /// <summary>
    /// Methods to help format text for TextElements
    /// </summary>
    public static class TextElementHelper
    {
        /// <summary>
        /// Generates a pattern based on an elements properties
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GenerateTextPattern<T>(T element) where T : ViewElement, ITextElement
        {
            string[] characterPattern = new string[element.Height];

            string[] rows;
            if (!element.WrapText || element.Text.Length <= element.Width)
            {
                rows = new[] { Pad(element, element.Text) };
            }
            else
            {
                rows = new string[(element.Text.Length + element.Width - 1) / element.Width];

                int i;
                for (i = 0; i < rows.Length - 1; ++i)
                    rows[i] = Pad(element, element.Text.Substring(i * element.Width, element.Width));

                rows[i] = Pad(element, element.Text.Substring(i * element.Width, element.Text.Length - i * element.Width));
            }

            if (rows.Length >= element.Height)
            {
                for (int i = 0; i < element.Height; ++i)
                    characterPattern[i] = rows[i];
            }
            else
            {
                int paddingTop = 0;
                switch (element.TextAlignmentVertical)
                {
                    case AlignmentVertical.Middle:
                        paddingTop = Math.Max(0, element.Height - rows.Length) / 2;
                        break;
                    case AlignmentVertical.Bottom:
                        paddingTop = Math.Max(0, element.Height - rows.Length);
                        break;
                }

                string emptyRow = new string(element.Filler, element.Width);

                int labelRow;
                for (labelRow = 0; labelRow < paddingTop; ++labelRow)
                    characterPattern[labelRow] = emptyRow;

                for (int textRow = 0; textRow < rows.Length; ++labelRow, ++textRow)
                    characterPattern[labelRow] = rows[textRow];

                for (; labelRow < element.Height; ++labelRow)
                    characterPattern[labelRow] = emptyRow;
            }

            return characterPattern;
        }

        /// <summary>
        /// Generates a pattern based on an elements properties
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text">The text to use in place of the elements own Text property</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GenerateTextPattern<T>(T element, string text) where T : ViewElement, ITextElement
        {
            string[] characterPattern = new string[element.Height];

            string[] rows;
            if (!element.WrapText || text.Length <= element.Width)
            {
                rows = new[] { Pad(element, text) };
            }
            else
            {
                rows = new string[(text.Length + element.Width - 1) / element.Width];

                int i;
                for (i = 0; i < rows.Length - 1; ++i)
                    rows[i] = Pad(element, text.Substring(i * element.Width, element.Width));

                rows[i] = Pad(element, text.Substring(i * element.Width, text.Length - i * element.Width));
            }

            if (rows.Length >= element.Height)
            {
                for (int i = 0; i < element.Height; ++i)
                    characterPattern[i] = rows[i];
            }
            else
            {
                int paddingTop = 0;
                switch (element.TextAlignmentVertical)
                {
                    case AlignmentVertical.Middle:
                        paddingTop = Math.Max(0, element.Height - rows.Length) / 2;
                        break;
                    case AlignmentVertical.Bottom:
                        paddingTop = Math.Max(0, element.Height - rows.Length);
                        break;
                }

                string emptyRow = new string(element.Filler, element.Width);

                int labelRow;
                for (labelRow = 0; labelRow < paddingTop; ++labelRow)
                    characterPattern[labelRow] = emptyRow;

                for (int textRow = 0; textRow < rows.Length; ++labelRow, ++textRow)
                    characterPattern[labelRow] = rows[textRow];

                for (; labelRow < element.Height; ++labelRow)
                    characterPattern[labelRow] = emptyRow;
            }

            return characterPattern;
        }

        private static string Pad<T>(T element, string text) where T : ViewElement, ITextElement
        {
            if (text.Length > element.Width)
                return text.Substring(0, element.Width);

            if (text.Length == element.Width)
                return text;

            string paddingLeft = "";
            string paddingRight = "";
            switch (element.TextAlignmentHorizontal)
            {
                case AlignmentHorizontal.Left:
                    paddingRight = new string(element.Filler, element.Width - text.Length);
                    break;
                case AlignmentHorizontal.Middle:
                    paddingLeft = new string(element.Filler, (element.Width - text.Length) / 2);
                    paddingRight = new string(element.Filler, element.Width - text.Length - paddingLeft.Length);
                    break;
                case AlignmentHorizontal.Right:
                    paddingLeft = new string(element.Filler, element.Width - text.Length);
                    break;
            }

            return string.Concat(paddingLeft, text, paddingRight);
        }
    }
}
