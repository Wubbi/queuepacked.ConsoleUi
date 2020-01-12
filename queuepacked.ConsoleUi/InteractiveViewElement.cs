using System;

namespace queuepacked.ConsoleUI
{
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
}