namespace queuepacked.ConsoleUI
{
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
}