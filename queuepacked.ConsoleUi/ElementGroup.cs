using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Arranges elements together
    /// </summary>
    public abstract class ElementGroup : IElement
    {
        private int _x;
        private int _y;

        // ReSharper disable once InconsistentNaming
        private protected int _width;
        // ReSharper disable once InconsistentNaming
        private protected int _height;

        /// <inheritdoc cref="IElement.X"/>
        public int X
        {
            get => _x;
            set
            {
                if (_x == value)
                    return;

                PropertyChanged(_x, value);
                _x = value;
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

                PropertyChanged(_y, value);
                _y = value;
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

                PropertyChanged(_width, value);
                _width = value;
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

                PropertyChanged(_height, value);
                _height = value;
            }
        }

        private protected readonly List<IElement> Elements;

        /// <summary>
        /// Creates a new instance of <see cref="ElementGroup"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected ElementGroup(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;

            Elements = new List<IElement>();
        }

        /// <summary>
        /// Adds an element to this group
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if the element was added, false if it already was part of this group</returns>
        public bool AddElement(IElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            if (Elements.Contains(element))
                return false;

            Elements.Add(element);
            return true;
        }

        /// <summary>
        /// Adds several elements to this group
        /// </summary>
        /// <param name="elements"></param>
        public void AddElements(params IElement[] elements)
        {
            foreach (IElement element in elements)
                AddElement(element);
        }

        /// <summary>
        /// Removes an element from this group
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if the element was removed, false if it was not part of this group</returns>
        public bool RemoveElement(IElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return Elements.Remove(element);
        }

        private void PropertyChanged(int oldValue, int newValue, [CallerMemberName] string propertyName = "")
        {
            int diff = newValue - oldValue;
            switch (propertyName)
            {
                case nameof(X):
                    foreach (IElement element in Elements)
                        element.X += diff;
                    break;
                case nameof(Y):
                    foreach (IElement element in Elements)
                        element.Y += diff;
                    break;
                default:
                    Reorder();
                    break;
            }
        }

        /// <summary>
        /// Rearranges he contained elements
        /// </summary>
        public abstract void Reorder();
    }
}
