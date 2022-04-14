// Quarrel © 2022

using Quarrel.Services.Keyboard.Enums;

namespace Quarrel.Services.Keyboard
{
    public struct KeyboardShortcut
    {
        public KeyboardShortcut(Key key, KeyModifiers modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public Key Key { get; }

        public KeyModifiers Modifiers { get; }
    }
}
