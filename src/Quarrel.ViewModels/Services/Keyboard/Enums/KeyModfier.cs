// Quarrel © 2022

using System;

namespace Quarrel.Services.Keyboard.Enums
{
    /// <summary>
    /// The modifer keys being pressed.
    /// </summary>
    [Flags]
    public enum KeyModifiers : uint
    {
        /// <summary>
        /// No modification keys are active.
        /// </summary>
        None = 0x0u,

        /// <summary>
        /// The Ctrl (control) virtual key.
        /// </summary>
        Control = 0x1,

        /// <summary>
        /// The Menu key.
        /// </summary>
        Menu = 0x2,

        /// <summary>
        /// The Shift key.
        /// </summary>
        Shift = 0x4,

        /// <summary>
        /// The Windows key.
        /// </summary>
        Windows = 0x8,
    }
}
