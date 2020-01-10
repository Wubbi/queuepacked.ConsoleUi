using System;
using NUnit.Framework;
using queuepacked.ConsoleUI.ViewElements;

namespace queuepacked.ConsoleUI.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test_Symbol_Compare()
        {
            Symbol a = new Symbol(' ', ConsoleColor.Black, ConsoleColor.Gray);
            Symbol b = new Symbol(' ', ConsoleColor.Black, ConsoleColor.Gray);
            Symbol c = new Symbol('-', ConsoleColor.Black, ConsoleColor.Gray);

            Assert.True(a.Equals(b));
            Assert.False(a.Equals(c));
        }

        [Test]
        public void Test_Pattern_Builder_Create()
        {
            Pattern.Builder builder = Pattern.Builder.New(3, 2);

            builder.AddSymbols(0, 0, new Symbol('a', ConsoleColor.Black, ConsoleColor.Gray));

            builder.AddText(0, 1, "bc", ConsoleColor.Blue, ConsoleColor.Cyan);

            builder.Rect(2, 0, 1, 2, new Symbol('#', ConsoleColor.DarkGray, ConsoleColor.Yellow));

            Pattern pattern = builder.Create();


            Assert.AreEqual(3, pattern.Width);
            Assert.AreEqual(2, pattern.Height);

            Assert.AreEqual('a', pattern[0, 0].Character);
            Assert.AreEqual(ConsoleColor.Black, pattern[0, 0].BackgroundColor);
            Assert.AreEqual(ConsoleColor.Gray, pattern[0, 0].ForegroundColor);

            Assert.AreEqual('b', pattern[0, 1].Character);
            Assert.AreEqual(ConsoleColor.Blue, pattern[0, 1].BackgroundColor);
            Assert.AreEqual(ConsoleColor.Cyan, pattern[0, 1].ForegroundColor);

            Assert.AreEqual('c', pattern[1, 1].Character);
            Assert.AreEqual(ConsoleColor.Blue, pattern[1, 1].BackgroundColor);
            Assert.AreEqual(ConsoleColor.Cyan, pattern[1, 1].ForegroundColor);

            Assert.AreEqual(Buffer.Invisible, pattern[1, 0].Character);
            Assert.AreEqual(ConsoleColor.Black, pattern[1, 0].BackgroundColor);
            Assert.AreEqual(ConsoleColor.Black, pattern[1, 0].ForegroundColor);

            Assert.AreEqual('#', pattern[2, 0].Character);
            Assert.AreEqual(ConsoleColor.DarkGray, pattern[2, 0].BackgroundColor);
            Assert.AreEqual(ConsoleColor.Yellow, pattern[2, 0].ForegroundColor);

            Assert.AreEqual('#', pattern[2, 1].Character);
            Assert.AreEqual(ConsoleColor.DarkGray, pattern[2, 1].BackgroundColor);
            Assert.AreEqual(ConsoleColor.Yellow, pattern[2, 1].ForegroundColor);
        }

        [Test]
        public void Test_Pattern_Builder_Limits()
        {
            Pattern.Builder builder;

            Assert.Throws<ArgumentOutOfRangeException>(() => builder = Pattern.Builder.New(0, 0));

            builder = Pattern.Builder.New(1, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddText(-1, 0, "", ConsoleColor.Black, ConsoleColor.Gray));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddText(0, 2, "", ConsoleColor.Black, ConsoleColor.Gray));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddText(0, 0, "abc", ConsoleColor.Black, ConsoleColor.Gray));
            Assert.Throws<ArgumentNullException>(() => builder.AddText(0, 0, null, ConsoleColor.Black, ConsoleColor.Gray));
        }

        [Test]
        public void Test_Buffer()
        {
            Buffer buffer = new Buffer(0, 0, 10);

            buffer.Rect(0, 0, 4, 3, new Symbol('R', ConsoleColor.Red, ConsoleColor.DarkBlue));

            buffer.Pattern(1, 1, Pattern.Builder.New(1, 1).Create());

            buffer.Symbols(0, 4, new Symbol('l', ConsoleColor.Black, ConsoleColor.Gray), new Symbol('L', ConsoleColor.Black, ConsoleColor.Gray));

            buffer.Clear(new Symbol('E', ConsoleColor.DarkRed, ConsoleColor.Magenta));
        }

        [Test]
        public void Test_InputCatcher()
        {
            InputCatcher inputCatcher = new InputCatcher();

            inputCatcher.SetInput(InputType.Enter, new KeyModifierCombo(ConsoleKey.E, ConsoleModifiers.Shift), new KeyModifierCombo(ConsoleKey.B, ConsoleModifiers.Control));
            inputCatcher.SetInput(InputType.Generic, new KeyModifierCombo(ConsoleKey.C));
        }

        [Test]
        public void Test_View()
        {
            View view = new View("TestView", 0, 20, 10, new Symbol(' ', ConsoleColor.Black, ConsoleColor.Gray));

            view.AddElement(new Label(0, 0, ""));

            view.Redraw();

            view.AddElement(new Label(0, 0, ""));

            view.Refresh(true);

            view.AddElements(new Label(0, 0, ""), new Label(0, 1, ""));
        }

        [Test]
        public void Test_Label()
        {
            Label label = new Label(0, 0, 0, 0, "Label") { TextAlignmentHorizontal = AlignmentHorizontal.Middle };
        }

        [Test]
        public void Test_Button()
        {
            Button button = new Button(0, 0, 1, 1, "Button") { WrapText = true };
        }

        [Test]
        public void Test_RotoList()
        {
            RotoList<int> rotoList = new RotoList<int>(0, 0, 10, 10);

            rotoList.SetElements(new[] { new RotoList<int>.Element<int>("A", 1), new RotoList<int>.Element<int>("B", 2) });

            RotoList<int>.Element<int> rotoListCurrentElement = rotoList.CurrentElement;

            Assert.AreEqual("A", rotoListCurrentElement.Name);
            Assert.AreEqual(1, rotoListCurrentElement.Value);
        }

        [Test]
        public void Test_Rectangle()
        {
            Rectangle rectangle = new Rectangle(0, 0, 4, 5, 1);
        }

        [Test]
        public void Test_ElementList()
        {
            ElementList elementList = new ElementList(0, 0, true);
            Label a = new Label(0, 0, "A");
            Label b = new Label(0, 0, "B");
            Label c = new Label(0, 0, "C");

            elementList.AddElement(a);
            elementList.AddElements(b, c);

            elementList.Reorder();

            Assert.AreEqual(0, a.X);
            Assert.AreEqual(1, b.X);
            Assert.AreEqual(2, c.X);

            elementList.X = 1;

            Assert.AreEqual(1, a.X);
            Assert.AreEqual(2, b.X);
            Assert.AreEqual(3, c.X);

            elementList.Horizontal = false;

            Assert.AreEqual(1, a.X);
            Assert.AreEqual(1, b.X);
            Assert.AreEqual(1, c.X);
            Assert.AreEqual(0, a.Y);
            Assert.AreEqual(1, b.Y);
            Assert.AreEqual(2, c.Y);

            elementList.Padding = 1;

            Assert.AreEqual(0, a.Y);
            Assert.AreEqual(2, b.Y);
            Assert.AreEqual(4, c.Y);
        }

        [Test]
        public void Test_ElementGrid()
        {
            ElementGrid grid = new ElementGrid(0, 0, 2, 2);
            Label a = new Label(0, 0, "A1");
            Label b = new Label(0, 0, "B");
            Label c = new Label(0, 0, "C");
            Rectangle d = new Rectangle(0, 0, 1, 2);

            grid.AddElement(a);
            grid.AddElements(b, c);
            grid.AddElements(d);

            grid.Reorder();

            Assert.AreEqual(4, grid.Width);
            Assert.AreEqual(4, grid.Height);

            Assert.AreEqual(0, a.X);
            Assert.AreEqual(2, b.X);
            Assert.AreEqual(0, c.X);
            Assert.AreEqual(2, d.X);

            Assert.AreEqual(0, a.Y);
            Assert.AreEqual(0, b.Y);
            Assert.AreEqual(2, c.Y);
            Assert.AreEqual(2, d.Y);
        }
    }
}
