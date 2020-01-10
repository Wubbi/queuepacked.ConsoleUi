namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Offers basic properties for arranging elements in a layout
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Gets or sets the X coordinate of this element
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of this element
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// Gets or sets the Width of this element
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the Height of this element
        /// </summary>
        int Height { get; set; }
    }
}
