using System;
using System.Collections.Generic;
using System.Threading;

namespace queuepacked.ConsoleUI
{
    /// <summary>
    /// Manages a collection of <see cref="View"/> objects and handles input
    /// </summary>
    public class UiHub : IDisposable
    {
        private static UiHub? _instance;
        
        private readonly ConsoleSettings _initialSettings;

        private readonly int _width;
        private readonly int _height;

        private readonly Dictionary<string, View> _views;

        private bool _running;

        private bool _disposed;

        private string _title;
        private View? _activeView;
        
        private readonly InputCatcher _inputCatcher;

        /// <summary>
        /// The currently active View
        /// </summary>
        public View? ActiveView
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                return _activeView;
            }
            private set => _activeView = value;
        }

        /// <summary>
        /// Gets or sets the title of the Console
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                _title = value;
                Console.Title = value;
            }
        }

        private UiHub(int width, int height, bool adjustWindow)
        {
            _initialSettings = new ConsoleSettings();

            _title = Console.Title;

            Console.CursorVisible = false;

            if (Console.BufferWidth < width)
                Console.BufferWidth = width;

            if (Console.BufferHeight < height)
                Console.BufferHeight = height;

            if (adjustWindow)
            {
                if (Console.WindowWidth != width)
                    Console.WindowWidth = width;

                if (Console.WindowHeight != height)
                    Console.WindowHeight = height;
            }

            Console.TreatControlCAsInput = true;

            _width = width;
            _height = height;

            _views = new Dictionary<string, View>();

            _disposed = false;

            _inputCatcher = new InputCatcher();

            _inputCatcher.SetInput(InputType.SelectionDown
                , new KeyModifierCombo(ConsoleKey.Tab, 0)
                , new KeyModifierCombo(ConsoleKey.DownArrow, 0)
            );

            _inputCatcher.SetInput(InputType.SelectionUp
                , new KeyModifierCombo(ConsoleKey.Tab, ConsoleModifiers.Shift)
                , new KeyModifierCombo(ConsoleKey.UpArrow, 0)
            );

            _inputCatcher.SetInput(InputType.Enter
                , new KeyModifierCombo(ConsoleKey.Enter, 0)
                , new KeyModifierCombo(ConsoleKey.Spacebar, 0)
            );

            _inputCatcher.SetInput(InputType.Left
                , new KeyModifierCombo(ConsoleKey.LeftArrow, 0)
            );

            _inputCatcher.SetInput(InputType.Right
                , new KeyModifierCombo(ConsoleKey.RightArrow, 0)
            );
        }

        /// <summary>
        /// Reset the Console settings to what they were when this <see cref="UiHub"/> was created
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _initialSettings.Set();

            Console.CursorTop = _initialSettings.CursorTop + _height;
            Console.CursorLeft = 0;

            _instance = null;
        }

        /// <summary>
        /// Continuously reads input and draws changes to the console
        /// </summary>
        public void Run()
        {
            if (_running)
                return;

            _running = true;

            while (_running)
            {
                Thread.Sleep(15);

                while (_inputCatcher.KeyPressed(out InputEventArgs inputEvent))
                    ActiveView?.OnNewInput(inputEvent);

                ActiveView?.DrawBuffer();
            }
        }

        /// <summary>
        /// Cancels an active call to <see cref="Run"/>
        /// </summary>
        public void Stop()
        {
            if (!_running)
                return;

            _running = false;
        }

        /// <summary>
        /// Creates a new <see cref="UiHub"/> at the current position of the cursor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="adjustWindow"></param>
        /// <returns></returns>
        public static UiHub Register(int width, int height, bool adjustWindow)
        {
            if (!(_instance is null))
                throw new Exception("A previous ViewSwitch instance was not disposed yet");

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _instance = new UiHub(width, height, adjustWindow);

            return _instance;
        }

        /// <summary>
        /// Creates a new <see cref="View"/>
        /// </summary>
        /// <param name="name">The unique name to identify the new View by</param>
        /// <returns></returns>
        public View AddView(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (_views.ContainsKey(name))
                throw new ArgumentException($"A View named \"{name}\" already exists");

            View view = new View(name, _initialSettings.CursorTop, _width, _height
                , new Symbol(' ', _initialSettings.BackgroundColor, _initialSettings.ForegroundColor));

            _views.Add(name, view);

            if (ActiveView is null)
                SwitchView(name);

            return view;
        }

        /// <summary>
        /// Switches to a different view
        /// </summary>
        /// <param name="name">The name of the View to switch to</param>
        /// <returns></returns>
        public View SwitchView(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (!_views.ContainsKey(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            ActiveView = _views[name];

            ActiveView.Refresh(true);

            return ActiveView;
        }
    }
}
