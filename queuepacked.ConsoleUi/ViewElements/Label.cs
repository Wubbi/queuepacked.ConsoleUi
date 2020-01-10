namespace queuepacked.ConsoleUI.ViewElements
{
    /// <summary>
    /// A simple <see cref="ViewElement"/> that can display text
    /// </summary>
    public class Label : ViewElement, ITextElement
    {
        private string _text;

        private AlignmentHorizontal _textAlignmentHorizontal;

        private AlignmentVertical _textAlignmentVertical;

        private bool _wrapText;

        private char _filler;

        private Pattern _pattern;

        /// <inheritdoc cref="ITextElement.Text"/>
        public string Text
        {
            get => _text;
            set
            {
                if (value is null)
                    value = "";

                if (_text.Equals(value))
                    return;

                _text = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="ITextElement.TextAlignmentHorizontal"/>
        public AlignmentHorizontal TextAlignmentHorizontal
        {
            get => _textAlignmentHorizontal;
            set
            {
                if (_textAlignmentHorizontal == value)
                    return;

                _textAlignmentHorizontal = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="ITextElement.TextAlignmentVertical"/>
        public AlignmentVertical TextAlignmentVertical
        {
            get => _textAlignmentVertical;
            set
            {
                if (_textAlignmentVertical == value)
                    return;

                _textAlignmentVertical = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="ITextElement.Filler"/>
        public char Filler
        {
            get => _filler;
            set
            {
                if (_filler == value)
                    return;

                _filler = value;
                PropertyChanged();
            }
        }

        /// <inheritdoc cref="ITextElement.WrapText"/>
        public bool WrapText
        {
            get => _wrapText;
            set
            {
                if (_wrapText == value)
                    return;

                _wrapText = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="Label"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Label(int x, int y, int width, int height) : this(x, y, width, height, "")
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Label"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        public Label(int x, int y, string text)
            : this(x, y, text?.Length ?? 0, 1, text, AlignmentHorizontal.Left, AlignmentVertical.Top)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Label"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="text"></param>
        public Label(int x, int y, int width, int height, string text)
            : this(x, y, width, height, text, AlignmentHorizontal.Left, AlignmentVertical.Top)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Label"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="text"></param>
        /// <param name="textAlignmentHorizontal"></param>
        /// <param name="textAlignmentVertical"></param>
        public Label(int x, int y, int width, int height, string text, AlignmentHorizontal textAlignmentHorizontal, AlignmentVertical textAlignmentVertical)
            : this(x, y, width, height, text, textAlignmentHorizontal, textAlignmentVertical, true)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Label"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="text"></param>
        /// <param name="textAlignmentHorizontal"></param>
        /// <param name="textAlignmentVertical"></param>
        /// <param name="wrapText"></param>
        public Label(int x, int y, int width, int height, string text, AlignmentHorizontal textAlignmentHorizontal, AlignmentVertical textAlignmentVertical, bool wrapText)
            : base(x, y, width < 1 ? 1 : width, height < 1 ? 1 : height)
        {
            _text = text ?? "";
            _textAlignmentHorizontal = textAlignmentHorizontal;
            _textAlignmentVertical = textAlignmentVertical;
            _wrapText = wrapText;
            _filler = ' ';
            UpdatePattern();
        }

        private void UpdatePattern()
        {
            string[] generateTextPattern = TextElementHelper.GenerateTextPattern(this);

            Pattern.Builder builder = Pattern.Builder.New(Width, Height);

            for (int i = 0; i < Height; ++i)
                builder.AddText(0, i, generateTextPattern[i], BackgroundColor, ForegroundColor);

            _pattern = builder.Create();
        }

        /// <inheritdoc cref="ViewElement.OnDraw"/>
        protected override void OnDraw(Buffer buffer)
        {
            buffer.Pattern(X, Y, _pattern);
        }

        /// <inheritdoc cref="ViewElement.OnPropertyChanged"/>
        protected override void OnPropertyChanged(string callerMember)
        {
            if (callerMember == nameof(X) || callerMember == nameof(Y))
                return;

            UpdatePattern();
        }
    }
}
