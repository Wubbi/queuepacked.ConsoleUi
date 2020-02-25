using queuepacked.ConsoleUI.ViewElements;

namespace queuepacked.ConsoleUI.Sandbox
{
    class Program
    {
        static void Main()
        {
            using UiHub uiHub = UiHub.Register(80, 25, true);

            uiHub.Title = "Demo";

            View mainView = uiHub.AddView("Main");

            ElementList elementList = new ElementList(30, 10, false);

            Label a = new Label(0, 0, "Hellooo");
            Label b = new Label(0, 0, "World");
            Label c = new Label(0, 0, "!!");

            Label d = new Label(20, 0, 50, 1);

            ElementGrid elementGrid = new ElementGrid(60, 2, 2, 2);

            Rectangle r1 = new Rectangle(0, 0, 3, 2) { Filler = '#' };
            Rectangle r2 = new Rectangle(0, 0, 1, 3) { Filler = '-' };
            Rectangle r3 = new Rectangle(0, 0, 1, 1);
            Rectangle r4 = new Rectangle(0, 0, 1, 1) { Filler = '0' };

            elementGrid.AddElements(r1, r2, r3, r4);
            elementGrid.Reorder();


            elementList.AddElements(a, b, c);

            elementList.Reorder();

            mainView.AddElements(a, b, c, d);
            mainView.AddElements(r1, r2, r3, r4);


            Label label = mainView.AddElement(new Label(0, 0, 10, 1));

            mainView.AddElement(new Button(10, 5, 20, 4, "Button 1")).Pressed += () =>
            {
                label.Text = "Pressed B1";
                elementList.Horizontal = !elementList.Horizontal;
            };
            mainView.AddElement(new Button(10, 10, 20, 4, "Button 2")).Pressed += () =>
            {
                label.Text = "Pressed B2";
                elementList.X = 30 + ((elementList.X + 1) % 10);
            };

            RotoList<int> rotoList = new RotoList<int>(0, 15, 20, 1);
            rotoList.ChangedElement += e =>
            {
                label.Text = "Switch " + e.Value.ToString();
                elementList.Padding = e.Value;
            };

            rotoList.SetElements(new[]
            {
                new RotoList<int>.Element<int>("One",1),
                new RotoList<int>.Element<int>("Two",2),
                new RotoList<int>.Element<int>("Three",3),
                new RotoList<int>.Element<int>("Pi",4),
            });

            mainView.AddElement(rotoList);

            uiHub.UnhandledKeyPress += (s, e) => d.Text = $"I: {e.KeyInfo.Modifiers}+{e.KeyInfo.Key}";

            uiHub.Run();
        }
    }
}
