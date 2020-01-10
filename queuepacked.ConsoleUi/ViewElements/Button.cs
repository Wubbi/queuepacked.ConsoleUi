using System;

namespace queuepacked.ConsoleUI.ViewElements
{
    /// <summary>
    /// An interactive element that can trigger actions
    /// </summary>
    public class Button : InteractiveViewElement, ITextElement
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
        /// Creates a new instance of <see cref="Button"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Button(int x, int y, int width, int height) : this(x, y, width, height, "")
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Button"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="text"></param>
        public Button(int x, int y, int width, int height, string text) : base(x, y, width, height)
        {
            _text = text ?? "";
            _textAlignmentHorizontal = AlignmentHorizontal.Middle;
            _textAlignmentVertical = AlignmentVertical.Middle;
            _wrapText = true;
            _filler = ' ';
            _pattern = Pattern.Builder.New(width, height).Create();
            
            UpdatePattern();
        }

        /// <summary>
        /// Triggered when this Button is triggered
        /// </summary>
        public event Action? Pressed;

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

        internal override void NewInput(InputEventArgs e)
        {
            if (e.InputType != InputType.Enter)
                return;

            e.ConsumedInput = true;
            Pressed?.Invoke();
        }

        private void UpdatePattern()
        {
            string[] generateTextPattern = TextElementHelper.GenerateTextPattern(this);

            Pattern.Builder builder = Pattern.Builder.New(Width, Height);

            for (int i = 0; i < Height; ++i)
                builder.AddText(0, i, generateTextPattern[i]
                    , Selected ? BackgroundColorSelected : BackgroundColor, Selected ? ForegroundColorSelected : ForegroundColor);

            _pattern = builder.Create();
        }
    }
}
