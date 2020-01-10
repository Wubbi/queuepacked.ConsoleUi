using System;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Arranges <see cref="IElement"/>s in a vertical or horizontal line
    /// </summary>
    public class ElementList : ElementGroup
    {
        private bool _horizontal;
        private AlignmentVertical _verticalAlignment;
        private AlignmentHorizontal _horizontalAlignment;
        private int _padding;

        /// <summary>
        /// Whether elements are arranged horizontally (true) or vertically (false)
        /// </summary>
        public bool Horizontal
        {
            get => _horizontal;
            set
            {
                if (_horizontal == value)
                    return;

                _horizontal = value;
                Reorder();
            }
        }

        /// <summary>
        /// Vertical alignment of elements. Ignored if <see cref="Horizontal"/> is False
        /// </summary>
        public AlignmentVertical VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (_verticalAlignment == value)
                    return;

                _verticalAlignment = value;
                Reorder();
            }
        }

        /// <summary>
        /// Horizontal alignment of elements. Ignored if <see cref="Horizontal"/> is True
        /// </summary>
        public AlignmentHorizontal HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                if (_horizontalAlignment == value)
                    return;

                _horizontalAlignment = value;
                Reorder();
            }
        }

        /// <summary>
        /// The padding between elements
        /// </summary>
        public int Padding
        {
            get => _padding;
            set
            {
                if (_padding == value)
                    return;

                _padding = value < 0 ? 0 : value;
                Reorder();
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ElementList"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="horizontal"></param>
        public ElementList(int x, int y, bool horizontal) : base(x, y, 0, 0)
        {
            Horizontal = horizontal;
            VerticalAlignment = AlignmentVertical.Middle;
            HorizontalAlignment = AlignmentHorizontal.Middle;
            Padding = 0;
        }

        /// <summary>
        /// Arranges this groups elements
        /// </summary>
        public override void Reorder()
        {
            if (Elements.Count == 0)
                return;

            int totalWidth = Horizontal ? (Elements.Count - 1) * Padding : 0;
            int totalHeight = Horizontal ? 0 : (Elements.Count - 1) * Padding;
            foreach (IElement element in Elements)
            {
                if (Horizontal)
                {
                    totalWidth += element.Width;
                    if (element.Height > totalHeight)
                        totalHeight = element.Height;
                }
                else
                {
                    totalHeight += element.Height;
                    if (element.Width > totalWidth)
                        totalWidth = element.Width;
                }
            }

            _width = totalWidth;
            _height = totalHeight;

            if (Horizontal)
            {
                int x = X;
                int verticalBuffer;
                foreach (IElement element in Elements)
                {
                    element.X = x;
                    x += element.Width + Padding;

                    verticalBuffer = totalHeight - element.Height;
                    switch (VerticalAlignment)
                    {
                        case AlignmentVertical.Middle:
                            element.Y = Y + verticalBuffer / 2;
                            break;
                        case AlignmentVertical.Bottom:
                            element.Y = Y + verticalBuffer;
                            break;
                        default:
                            element.Y = Y;
                            break;
                    }
                }
            }
            else
            {
                int y = Y;
                int horizontalBuffer;
                foreach (IElement element in Elements)
                {
                    element.Y = y;
                    y += element.Height + Padding;

                    horizontalBuffer = totalWidth - element.Width;
                    switch (HorizontalAlignment)
                    {
                        case AlignmentHorizontal.Middle:
                            element.X = X + horizontalBuffer / 2;
                            break;
                        case AlignmentHorizontal.Right:
                            element.X = X + horizontalBuffer;
                            break;
                        default:
                            element.X = X;
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Arranges <see cref="IElement"/>s in a matrix
    /// </summary>
    public class ElementGrid : ElementGroup
    {
        private AlignmentVertical _verticalAlignment;
        private AlignmentHorizontal _horizontalAlignment;
        private int _rows;
        private int _columns;

        /// <summary>
        /// The amount of rows in this Grid
        /// </summary>
        public int Rows
        {
            get => _rows;
            set
            {
                if (_rows == value)
                    return;

                _rows = value;
                Reorder();
            }
        }

        /// <summary>
        /// The amount of columns in this Grid
        /// </summary>
        public int Columns
        {
            get => _columns;
            set
            {
                if (_columns == value)
                    return;

                _columns = value;
                Reorder();
            }
        }

        /// <summary>
        /// Vertical alignment of elements
        /// </summary>
        public AlignmentVertical VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (_verticalAlignment == value)
                    return;

                _verticalAlignment = value;
                Reorder();
            }
        }

        /// <summary>
        /// Horizontal alignment of elements
        /// </summary>
        public AlignmentHorizontal HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                if (_horizontalAlignment == value)
                    return;

                _horizontalAlignment = value;
                Reorder();
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ElementList"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public ElementGrid(int x, int y, int columns, int rows) : base(x, y, 0, 0)
        {
            if (columns < 1)
                throw new ArgumentOutOfRangeException(nameof(columns));

            if (rows < 1)
                throw new ArgumentOutOfRangeException(nameof(rows));

            _columns = columns;
            _rows = rows;

            VerticalAlignment = AlignmentVertical.Middle;
            HorizontalAlignment = AlignmentHorizontal.Middle;
        }

        /// <summary>
        /// Arranges this groups elements
        /// </summary>
        public override void Reorder()
        {
            if (Elements.Count == 0)
                return;

            int columnCount = Elements.Count >= Columns ? Columns : Elements.Count;
            int rowCount = (Elements.Count + Columns - 1) / 2;

            int maxWidth = 0;
            int maxHeight = 0;

            foreach (IElement element in Elements)
            {
                if (element.Height > maxHeight)
                    maxHeight = element.Height;

                if (element.Width > maxWidth)
                    maxWidth = element.Width;
            }

            _width = maxWidth * columnCount;
            _height = maxHeight * rowCount;

            int i = 0;
            for (int y = 0; y < _height; y += maxHeight)
            {
                for (int x = 0; x < _width; x += maxWidth, ++i)
                {
                    IElement element = Elements[i];

                    switch (HorizontalAlignment)
                    {
                        case AlignmentHorizontal.Middle:
                            element.X = X + x + (maxWidth - element.Width) / 2;
                            break;
                        case AlignmentHorizontal.Right:
                            element.X = X + x + (maxWidth - element.Width);
                            break;
                        default:
                            element.X = X + x;
                            break;
                    }
                    switch (VerticalAlignment)
                    {
                        case AlignmentVertical.Middle:
                            element.Y = Y + y + (maxHeight - element.Height) / 2;
                            break;
                        case AlignmentVertical.Bottom:
                            element.Y = Y + y + (maxHeight - element.Height);
                            break;
                        default:
                            element.Y = Y + y;
                            break;
                    }

                }
            }
        }
    }
}
