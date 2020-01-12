using System;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Methods to help format text for TextElements
    /// </summary>
    public static class TextElementHelper
    {
        /// <summary>
        /// Generates a pattern based on an elements properties
        /// </summary>
        /// <param name="element"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GenerateTextPattern<T>(T element) where T : ViewElement, ITextElement
        {
            string[] characterPattern = new string[element.Height];

            string[] rows;
            if (!element.WrapText || element.Text.Length <= element.Width)
            {
                rows = new[] { Pad(element, element.Text) };
            }
            else
            {
                rows = new string[(element.Text.Length + element.Width - 1) / element.Width];

                int i;
                for (i = 0; i < rows.Length - 1; ++i)
                    rows[i] = Pad(element, element.Text.Substring(i * element.Width, element.Width));

                rows[i] = Pad(element, element.Text.Substring(i * element.Width, element.Text.Length - i * element.Width));
            }

            if (rows.Length >= element.Height)
            {
                for (int i = 0; i < element.Height; ++i)
                    characterPattern[i] = rows[i];
            }
            else
            {
                int paddingTop = 0;
                switch (element.TextAlignmentVertical)
                {
                    case AlignmentVertical.Middle:
                        paddingTop = Math.Max(0, element.Height - rows.Length) / 2;
                        break;
                    case AlignmentVertical.Bottom:
                        paddingTop = Math.Max(0, element.Height - rows.Length);
                        break;
                }

                string emptyRow = new string(element.Filler, element.Width);

                int labelRow;
                for (labelRow = 0; labelRow < paddingTop; ++labelRow)
                    characterPattern[labelRow] = emptyRow;

                for (int textRow = 0; textRow < rows.Length; ++labelRow, ++textRow)
                    characterPattern[labelRow] = rows[textRow];

                for (; labelRow < element.Height; ++labelRow)
                    characterPattern[labelRow] = emptyRow;
            }

            return characterPattern;
        }

        /// <summary>
        /// Generates a pattern based on an elements properties
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text">The text to use in place of the elements own Text property</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GenerateTextPattern<T>(T element, string text) where T : ViewElement, ITextElement
        {
            string[] characterPattern = new string[element.Height];

            string[] rows;
            if (!element.WrapText || text.Length <= element.Width)
            {
                rows = new[] { Pad(element, text) };
            }
            else
            {
                rows = new string[(text.Length + element.Width - 1) / element.Width];

                int i;
                for (i = 0; i < rows.Length - 1; ++i)
                    rows[i] = Pad(element, text.Substring(i * element.Width, element.Width));

                rows[i] = Pad(element, text.Substring(i * element.Width, text.Length - i * element.Width));
            }

            if (rows.Length >= element.Height)
            {
                for (int i = 0; i < element.Height; ++i)
                    characterPattern[i] = rows[i];
            }
            else
            {
                int paddingTop = 0;
                switch (element.TextAlignmentVertical)
                {
                    case AlignmentVertical.Middle:
                        paddingTop = Math.Max(0, element.Height - rows.Length) / 2;
                        break;
                    case AlignmentVertical.Bottom:
                        paddingTop = Math.Max(0, element.Height - rows.Length);
                        break;
                }

                string emptyRow = new string(element.Filler, element.Width);

                int labelRow;
                for (labelRow = 0; labelRow < paddingTop; ++labelRow)
                    characterPattern[labelRow] = emptyRow;

                for (int textRow = 0; textRow < rows.Length; ++labelRow, ++textRow)
                    characterPattern[labelRow] = rows[textRow];

                for (; labelRow < element.Height; ++labelRow)
                    characterPattern[labelRow] = emptyRow;
            }

            return characterPattern;
        }

        private static string Pad<T>(T element, string text) where T : ViewElement, ITextElement
        {
            if (text.Length > element.Width)
                return text.Substring(0, element.Width);

            if (text.Length == element.Width)
                return text;

            string paddingLeft = "";
            string paddingRight = "";
            switch (element.TextAlignmentHorizontal)
            {
                case AlignmentHorizontal.Left:
                    paddingRight = new string(element.Filler, element.Width - text.Length);
                    break;
                case AlignmentHorizontal.Middle:
                    paddingLeft = new string(element.Filler, (element.Width - text.Length) / 2);
                    paddingRight = new string(element.Filler, element.Width - text.Length - paddingLeft.Length);
                    break;
                case AlignmentHorizontal.Right:
                    paddingLeft = new string(element.Filler, element.Width - text.Length);
                    break;
            }

            return string.Concat(paddingLeft, text, paddingRight);
        }
    }
}