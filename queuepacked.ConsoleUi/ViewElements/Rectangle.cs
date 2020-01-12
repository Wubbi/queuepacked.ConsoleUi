namespace queuepacked.ConsoleUI.ViewElements
{
    /// <summary>
    /// A simple rectangle
    /// </summary>
    public class Rectangle : ViewElement
    {
        private int _thickness;
        private Pattern _pattern;
        private char _filler;

        /// <summary>
        /// The thickness of the rect, or 0 to fill it completely
        /// </summary>
        public int Thickness
        {
            get => _thickness;
            set
            {
                if (value < 0 || value >= (Width + 1) / 2 || value >= (Height + 1) / 2)
                    value = 0;

                if (_thickness == value)
                    return;

                _thickness = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// The character to use to fill this rectangle with
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(int x, int y, int width, int height) : this(x, y, width, height, 0)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thickness"></param>
        public Rectangle(int x, int y, int width, int height, int thickness) : base(x, y, width, height)
        {
            _thickness = thickness < 0 ? 0 : thickness;
            _filler = Buffer.Invisible;
            _pattern = Pattern.Empty;
            UpdatePattern();
        }

        private void UpdatePattern()
        {
            Pattern.Builder builder = Pattern.Builder.New(Width, Height);

            builder.Rect(0, 0, Width, Height, new Symbol(Filler, BackgroundColor, ForegroundColor));

            if (Thickness > 0)
                builder.Rect(Thickness, Thickness, Width - Thickness * 2, Height - Thickness * 2, new Symbol(Buffer.Invisible, BackgroundColor, ForegroundColor));

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
