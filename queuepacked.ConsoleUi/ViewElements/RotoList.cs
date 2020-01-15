using System;
using System.Collections.Generic;

namespace queuepacked.ConsoleUI.ViewElements
{
    /// <summary>
    /// An interactive element that can trigger actions
    /// </summary>
    public class RotoList<T> : InteractiveViewElement, ITextElement
    {
        private string _text;

        private AlignmentHorizontal _textAlignmentHorizontal;

        private AlignmentVertical _textAlignmentVertical;

        private bool _wrapText;

        private char _filler;

        private bool _border;

        private Pattern _pattern;

        private List<Element<T>> _elements;

        private int _currentIndex;

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
        /// Whether or not to render a border around ths button
        /// </summary>
        public bool Border
        {
            get => _border;
            set
            {
                if (_border == value)
                    return;

                _border = value;
                PropertyChanged();
            }
        }

        /// <summary>
        /// The currently selected <see cref="Element{T}"/>
        /// </summary>
        public Element<T> CurrentElement => _elements[_currentIndex];

        /// <summary>
        /// Creates a new instance of <see cref="RotoList{T}"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public RotoList(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _text = "";
            _textAlignmentHorizontal = AlignmentHorizontal.Middle;
            _textAlignmentVertical = AlignmentVertical.Middle;
            _wrapText = true;
            _filler = ' ';
            _pattern = Pattern.Empty;

            _elements = new List<Element<T>>();
            _currentIndex = -1;

            UpdatePattern();
        }

        /// <summary>
        /// Triggered when this RotoList is triggered
        /// </summary>
        public event Action<Element<T>>? ChangedElement;

        /// <summary>
        /// Sets the list of elements
        /// </summary>
        /// <param name="elements"></param>
        public void SetElements(IEnumerable<Element<T>> elements)
        {
            _elements = new List<Element<T>>(elements);
            SelectIndex(_elements.Count == 0 ? -1 : 0);
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

        internal override void NewInput(InputEventArgs e)
        {
            if (e.InputType != InputType.Left && e.InputType != InputType.Right && e.InputType != InputType.Enter)
                return;

            e.ConsumedInput = true;

            if (_elements.Count == 0)
                return;

            if (e.InputType == InputType.Left)
                SelectIndex((_currentIndex + _elements.Count - 1) % _elements.Count);
            else
                SelectIndex((_currentIndex + 1) % _elements.Count);
        }

        private void SelectIndex(int index)
        {
            _currentIndex = index;

            PropertyChanged(nameof(CurrentElement));
            ChangedElement?.Invoke(CurrentElement);
        }

        private void UpdatePattern()
        {
            if(_currentIndex<0)
                return;

            string[] generateTextPattern = TextElementHelper.GenerateTextPattern(this, CurrentElement.Name);

            if (Width <= 0 || Height <= 0)
            {
                _pattern = Pattern.Empty;
                return;
            }

            Pattern.Builder builder = Pattern.Builder.New(Width, Height);

            for (int i = 0; i < Height; ++i)
                builder.AddText(0, i, generateTextPattern[i]
                    , Selected ? BackgroundColorSelected : BackgroundColor, Selected ? ForegroundColorSelected : ForegroundColor);

            _pattern = builder.Create();
        }

        /// <summary>
        /// A single value in a <see cref="RotoList{T}"/>
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        public class Element<TE>
        {
            /// <summary>
            /// The value stored in this element
            /// </summary>
            public TE Value { get; }

            /// <summary>
            /// The name used to display in the console
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Creates a new instance of <see cref="Element{T}"/>
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public Element(string name, TE value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
