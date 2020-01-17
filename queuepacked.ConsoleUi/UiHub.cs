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
        internal static readonly bool IsWindows;

        static UiHub()
        {
            IsWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

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
        private int _mainLoopInterval;
        private int _viewUpdateReduction;
        private int _viewUpdateReductionCounter;

        /// <summary>
        /// Triggers when a running <see cref="UiHub"/> stops
        /// </summary>
        public event HubStopsEventHandler? HubStops;

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

                if (IsWindows)
                    Console.Title = value;
            }
        }

        /// <summary>
        /// Time in milliseconds the main loop waits for new inputs
        /// </summary>
        public int MainLoopInterval
        {
            get => _mainLoopInterval;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                if (value < 10)
                    value = 10;

                _mainLoopInterval = value;
            }
        }

        /// <summary>
        /// The amount of cycles of the main loop until the view is updated
        /// </summary>
        public int ViewUpdateReduction
        {
            get => _viewUpdateReduction;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(ToString());

                if (value < 1)
                    value = 1;

                _viewUpdateReduction = value;
            }
        }

        private UiHub(int width, int height, bool adjustWindow)
        {
            _initialSettings = new ConsoleSettings();

            _title = "";

            if (IsWindows)
            {
                _title = "";

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

            _mainLoopInterval = 30;
            _viewUpdateReduction = 2;
            _viewUpdateReductionCounter = 0;
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

            Console.SetCursorPosition(0, _initialSettings.CursorTop + _height);

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
                Thread.Sleep(_mainLoopInterval);

                while (_inputCatcher.KeyPressed(out InputEventArgs inputEvent))
                {
                    ActiveView?.OnNewInput(inputEvent);

                    if (!inputEvent.ConsumedInput && inputEvent.KeyInfo.Key == ConsoleKey.C && inputEvent.KeyInfo.Modifiers == ConsoleModifiers.Control)
                    {
                        HubStopsEventArgs eventArgs = new HubStopsEventArgs();
                        HubStops?.Invoke(this, eventArgs);
                        if (eventArgs.Stop)
                            Stop();
                    }
                }

                ++_viewUpdateReductionCounter;
                if (_viewUpdateReductionCounter < _viewUpdateReduction)
                    continue;

                _viewUpdateReductionCounter = 0;

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
                throw new Exception("A previous UiHub instance was not yet disposed of");

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

    /// <summary>
    /// Handles the stopping event of a <see cref="UiHub"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void HubStopsEventHandler(UiHub sender, HubStopsEventArgs eventArgs);

    /// <summary>
    /// Contains properties detailing a shutdown of a <see cref="UiHub"/>
    /// </summary>
    public class HubStopsEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the UiHub should proceed with stopping. If set to false a stop event is ignored
        /// </summary>
        public bool Stop { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        internal HubStopsEventArgs()
        {
            Stop = true;
        }
    }
}
