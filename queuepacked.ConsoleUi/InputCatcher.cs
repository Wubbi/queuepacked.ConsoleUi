using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace queuepacked.ConsoleUI
{
    internal class InputCatcher
    {
        private readonly ReadOnlyCollection<InputType> _specialInputTypes;
        private readonly Dictionary<InputType, List<KeyModifierCombo>> _keyMapping;

        internal InputCatcher()
        {
            _specialInputTypes = new ReadOnlyCollection<InputType>(new List<InputType>
            {
                InputType.Enter,
                InputType.SelectionDown,
                InputType.SelectionUp,
                InputType.Left,
                InputType.Right
            });

            _keyMapping = _specialInputTypes.ToDictionary(v => v, v => new List<KeyModifierCombo>());
        }

        internal void SetInput(InputType inputTypeType, params KeyModifierCombo[] keyCombo)
        {
            _keyMapping[inputTypeType] = new List<KeyModifierCombo>(keyCombo);
        }

        internal bool KeyPressed(out InputEventArgs inputEvent)
        {
            inputEvent = InputEventArgs.Empty;

            if (!Console.KeyAvailable)
                return false;

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            InputType inputType = InputType.Generic;

            foreach (InputType specialInput in _specialInputTypes)
            {
                if (!_keyMapping[specialInput].Any(k => k.Matches(keyInfo)))
                    continue;

                inputType |= specialInput;
            }

            inputEvent = new InputEventArgs(inputType, keyInfo);

            return true;
        }
    }

    [Flags]
    internal enum InputType
    {
        Generic = 0,
        Enter = 1,
        SelectionUp = 2,
        SelectionDown = 4,
        Left = 8,
        Right = 16
    }

    internal struct KeyModifierCombo
    {
        public readonly ConsoleKey Key;
        public readonly ConsoleModifiers Modifier;

        public KeyModifierCombo(ConsoleKey key, ConsoleModifiers modifier)
        {
            Key = key;
            Modifier = modifier;
        }

        public KeyModifierCombo(ConsoleKey key)
        {
            Key = key;
            Modifier = 0;
        }

        public bool Matches(ConsoleKeyInfo keyInfo)
            => keyInfo.Key == Key && keyInfo.Modifiers == Modifier;
    }

    internal class InputEventArgs : EventArgs
    {
        public new static readonly InputEventArgs Empty;

        static InputEventArgs()
        {
            Empty = new InputEventArgs(InputType.Generic, new ConsoleKeyInfo()) { ConsumedInput = true };
        }

        public bool ConsumedInput { get; set; }

        public InputType InputType { get; }

        public ConsoleKeyInfo KeyInfo { get; }

        public InputEventArgs(InputType inputType, ConsoleKeyInfo keyInfo)
        {
            InputType = inputType;
            KeyInfo = keyInfo;
            ConsumedInput = false;
        }
    }

    internal delegate void InputEventHandler(InputEventArgs e);
}
