using System;
using System.Collections.Generic;
using System.Linq;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Represents a single arrangement of UI elements tied directly to an area in the Console
    /// </summary>
    public class View
    {
        private readonly Symbol _clearance;
        private readonly Buffer _buffer;

        private readonly List<ViewElement> _elements;

        private InteractiveViewElement? _selectedElement;

        private readonly object _elementRedrawLock;

        private bool? _bufferUpdate;

        /// <summary>
        /// The name of this <see cref="View"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new instance of <see cref="View"/>
        /// </summary>
        /// <param name="name">The name to reference this view with</param>
        /// <param name="top">The top position of this <see cref="View"/> in the console</param>
        /// <param name="width">The total width this view should have.</param>
        /// <param name="height">The total height this view should have</param>
        /// <param name="clearance">The filler character used to clear this view</param>
        internal View(string name, int top, int width, int height, Symbol clearance)
        {
            _clearance = clearance;
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _buffer = new Buffer(top, width, height);

            _elements = new List<ViewElement>();

            _elementRedrawLock = new object();
        }

        /// <summary>
        /// Adds a <see cref="ViewElement"/> to this View
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public T AddElement<T>(T element) where T : ViewElement
        {
            if (_elements.Contains(element))
                throw new ArgumentException("The element was already added");

            _elements.Add(element);

            ElementOnRedrawRequired(element);

            element.RedrawRequired += ElementOnRedrawRequired;

            return element;
        }

        /// <summary>
        /// Adds several <see cref="ViewElement"/>s to this View
        /// </summary>
        /// <param name="elements"></param>
        public void AddElements(params ViewElement[] elements)
        {
            foreach (ViewElement viewElement in elements)
                AddElement(viewElement);
        }

        /// <summary>
        /// Draws the current elements to the buffer and triggers a <see cref="Refresh"/>
        /// </summary>
        public void Redraw()
        {
            _buffer.Clear(_clearance);

            foreach (ViewElement element in _elements.Where(e => e.Visible))
                element.Draw(_buffer);

            Refresh(false);
        }

        /// <summary>
        /// Draws the buffer to the console
        /// </summary>
        public void Refresh(bool fullRedraw)
        {
            if (!_bufferUpdate.HasValue)
                _bufferUpdate = fullRedraw;
            else if (fullRedraw && !_bufferUpdate.Value)
                _bufferUpdate = true;
        }

        internal void OnNewInput(InputEventArgs e)
        {
            _selectedElement?.NewInput(e);

            if (e.ConsumedInput)
                return;

            List<InteractiveViewElement> interactiveViewElements = _elements.Where(el => el is InteractiveViewElement ie && ie.Enabled && ie.Visible)
                                                                            .Cast<InteractiveViewElement>().OrderBy(i => i.Tabindex).ToList();

            if (e.InputType.HasFlag(InputType.SelectionDown))
            {
                if (interactiveViewElements.Count == 0)
                {
                    SelectElement(null);
                    return;
                }

                int nextElementIndex = (_selectedElement is null ? -1 : interactiveViewElements.IndexOf(_selectedElement)) + 1;

                if (nextElementIndex >= interactiveViewElements.Count)
                    nextElementIndex = 0;

                SelectElement(interactiveViewElements[nextElementIndex]);
            }
            else if (e.InputType.HasFlag(InputType.SelectionUp))
            {
                if (interactiveViewElements.Count == 0)
                {
                    SelectElement(null);
                    return;
                }

                int nextElementIndex = (_selectedElement is null ? -1 : interactiveViewElements.IndexOf(_selectedElement)) - 1;

                if (nextElementIndex < 0)
                    nextElementIndex = interactiveViewElements.Count - 1;

                SelectElement(interactiveViewElements[nextElementIndex]);
            }
        }

        internal void DrawBuffer()
        {
            if (!_bufferUpdate.HasValue)
                return;

            _buffer.Draw(_bufferUpdate.Value);
            _bufferUpdate = null;
        }

        private void ElementOnRedrawRequired(ViewElement obj)
        {
            lock (_elementRedrawLock)
            {
                if (!obj.Dirty)
                    return;

                if (obj is InteractiveViewElement e && !(e.Enabled && e.Visible))
                    SelectElement(null);

                Redraw();
            }
        }

        private void SelectElement(InteractiveViewElement? element)
        {
            if (_selectedElement != null)
                _selectedElement.Selected = false;

            if (element is null || !_elements.Contains(element))
            {
                _selectedElement = null;
                return;
            }

            _selectedElement = element;
            _selectedElement.Selected = true;
        }
    }
}
