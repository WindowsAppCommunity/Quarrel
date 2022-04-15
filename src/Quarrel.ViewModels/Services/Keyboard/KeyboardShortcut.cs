// Quarrel © 2022

using Quarrel.Services.Keyboard.Enums;

namespace Quarrel.Services.Keyboard
{
    /// <summary>
    /// A keyboard shortcut key combindation.
    /// </summary>
    public struct KeyboardShortcut
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardShortcut"/> struct.
        /// </summary>
        /// <param name="key">The main key pressed.</param>
        /// <param name="modifiers">The modifier keys pressed along with it.</param>
        public KeyboardShortcut(Key key, KeyModifiers modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the main key in the shortcut.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Gets the modifer keys in the shortcut.
        /// </summary>
        public KeyModifiers Modifiers { get; }
    }
}
