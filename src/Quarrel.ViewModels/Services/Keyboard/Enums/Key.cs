// Adam Dernis © 2022

namespace Quarrel.Services.Keyboard.Enums
{
    /// <summary>
    /// The values for virtual keys.
    /// </summary>
    public enum Key
    {
        /// <summary>
        /// No key value.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button.
        /// </summary>
        LeftButton = 1,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        RightButton = 2,

        /// <summary>
        /// The cancel key or button
        /// </summary>
        Cancel = 3,

        /// <summary>
        /// The middle mouse button.
        /// </summary>
        MiddleButton = 4,

        /// <summary>
        /// An additional "extended" device key or button (for example, an additional mouse button).
        /// </summary>
        XButton1 = 5,

        /// <summary>
        /// An additional "extended" device key or button (for example, an additional mouse button).
        /// </summary>
        XButton2 = 6,

        /// <summary>
        /// The back key or button.
        /// </summary>
        Back = 8,

        /// <summary>
        /// The Tab key.
        /// </summary>
        Tab = 9,

        /// <summary>
        /// The Clear key or button.
        /// </summary>
        Clear = 12,

        /// <summary>
        /// The Enter key.
        /// </summary>
        Enter = 13,

        /// <summary>
        /// The Shift key. This is the general Shift case, applicable to key layouts with
        /// only one Shift key or that do not need to differentiate between left Shift and
        /// right Shift keystrokes.
        /// </summary>
        Shift = 0x10,

        /// <summary>
        /// The Ctrl key. This is the general Ctrl case, applicable to key layouts with only
        /// one Ctrl key or that do not need to differentiate between left Ctrl and right
        /// Ctrl keystrokes.
        /// </summary>
        Control = 17,

        /// <summary>
        /// The menu key or button.
        /// </summary>
        Menu = 18,

        /// <summary>
        /// The Pause key or button.
        /// </summary>
        Pause = 19,

        /// <summary>
        /// The Caps Lock key or button.
        /// </summary>
        CapitalLock = 20,


        /// <summary>
        /// The Kana symbol key-shift button
        /// </summary>
        Kana = 21,


        /// <summary>
        /// The Hangul symbol key-shift button.
        /// </summary>
        Hangul = 21,

        /// <summary>
        /// The ImeOn symbol key-shift button.
        /// </summary>
        ImeOn = 22,

        /// <summary>
        /// The Junja symbol key-shift button.
        /// </summary>
        Junja = 23,

        /// <summary>
        /// The Final symbol key-shift button.
        /// </summary>
        Final = 24,

        /// <summary>
        /// The Hanja symbol key shift button.
        /// </summary>
        Hanja = 25,

        /// <summary>
        /// The Kanji symbol key-shift button.
        /// </summary>
        Kanji = 25,

        /// <summary>
        /// The ImeOff symbol key-shift button.
        /// </summary>
        ImeOff = 26,

        /// <summary>
        /// The Esc key.
        /// </summary>
        Escape = 27,

        /// <summary>
        /// The convert button or key.
        /// </summary>
        Convert = 28,

        /// <summary>
        /// The nonconvert button or key.
        /// </summary>
        NonConvert = 29,

        /// <summary>
        /// The accept button or key.
        /// </summary>
        Accept = 30,

        /// <summary>
        /// The mode change key.
        /// </summary>
        ModeChange = 0x1F,

        /// <summary>
        /// The Spacebar key or button.
        /// </summary>
        Space = 0x20,

        /// <summary>
        /// The Page Up key.
        /// </summary>
        PageUp = 33,

        /// <summary>
        /// The Page Down key.
        /// </summary>
        PageDown = 34,

        /// <summary>
        /// The End key.
        /// </summary>
        End = 35,

        /// <summary>
        /// The Home key.
        /// </summary>
        Home = 36,

        /// <summary>
        /// The Left Arrow key.
        /// </summary>
        Left = 37,

        /// <summary>
        /// The Up Arrow key.
        /// </summary>
        Up = 38,

        /// <summary>
        /// The Right Arrow key.
        /// </summary>
        Right = 39,

        /// <summary>
        /// The Down Arrow key.
        /// </summary>
        Down = 40,

        /// <summary>
        /// The Select key or button.
        /// </summary>
        Select = 41,

        /// <summary>
        /// The Print key or button.
        /// </summary>
        Print = 42,

        /// <summary>
        /// The execute key or button.
        /// </summary>
        Execute = 43,

        /// <summary>
        /// The snapshot key or button.
        /// </summary>
        Snapshot = 44,

        /// <summary>
        /// The Insert key.
        /// </summary>
        Insert = 45,

        /// <summary>
        /// The Delete key.
        /// </summary>
        Delete = 46,

        /// <summary>
        /// The Help key or button.
        /// </summary>
        Help = 47,

        /// <summary>
        /// The number "0" key.
        /// </summary>
        Number0 = 48,

        /// <summary>
        /// The number "1" key.
        /// </summary>
        Number1 = 49,

        /// <summary>
        /// The number "2" key.
        /// </summary>
        Number2 = 50,

        /// <summary>
        /// The number "3" key.
        /// </summary>
        Number3 = 51,

        /// <summary>
        /// The number "4" key.
        /// </summary>
        Number4 = 52,

        /// <summary>
        /// The number "5" key.
        /// </summary>
        Number5 = 53,

        /// <summary>
        /// The number "6" key.
        /// </summary>
        Number6 = 54,

        /// <summary>
        /// The number "7" key.
        /// </summary>
        Number7 = 55,

        /// <summary>
        /// The number "8" key.
        /// </summary>
        Number8 = 56,

        /// <summary>
        /// The number "9" key.
        /// </summary>
        Number9 = 57,

        /// <summary>
        /// The letter "A" key.
        /// </summary>
        A = 65,

        /// <summary>
        /// The letter "B" key.
        /// </summary>
        B = 66,

        /// <summary>
        /// The letter "C" key.
        /// </summary>
        C = 67,

        /// <summary>
        /// The letter "D" key.
        /// </summary>
        D = 68,

        /// <summary>
        /// The letter "E" key.
        /// </summary>
        E = 69,

        /// <summary>
        /// The letter "F" key.
        /// </summary>
        F = 70,

        /// <summary>
        /// The letter "G" key.
        /// </summary>
        G = 71,

        /// <summary>
        /// The letter "H" key.
        /// </summary>
        H = 72,

        /// <summary>
        /// The letter "I" key.
        /// </summary>
        I = 73,

        /// <summary>
        /// The letter "J" key.
        /// </summary>
        J = 74,

        /// <summary>
        /// The letter "K" key.
        /// </summary>
        K = 75,

        /// <summary>
        /// The letter "L" key.
        /// </summary>
        L = 76,

        /// <summary>
        /// The letter "M" key.
        /// </summary>
        M = 77,

        /// <summary>
        /// The letter "N" key.
        /// </summary>
        N = 78,

        /// <summary>
        /// The letter "O" key.
        /// </summary>
        O = 79,

        /// <summary>
        /// The letter "P" key.
        /// </summary>
        P = 80,

        /// <summary>
        /// The letter "Q" key.
        /// </summary>
        Q = 81,

        /// <summary>
        /// The letter "R" key.
        /// </summary>
        R = 82,

        /// <summary>
        /// The letter "S" key.
        /// </summary>
        S = 83,

        /// <summary>
        /// The letter "T" key.
        /// </summary>
        T = 84,

        /// <summary>
        /// The letter "U" key.
        /// </summary>
        U = 85,

        /// <summary>
        /// The letter "V" key.
        /// </summary>
        V = 86,

        /// <summary>
        /// The letter "W" key.
        /// </summary>
        W = 87,

        /// <summary>
        /// The letter "X" key.
        /// </summary>
        X = 88,

        /// <summary>
        /// The letter "Y" key.
        /// </summary>
        Y = 89,

        /// <summary>
        /// The letter "Z" key.
        /// </summary>
        Z = 90,

        /// <summary>
        /// The left Windows key.
        /// </summary>
        LeftWindows = 91,

        /// <summary>
        /// The right Windows key.
        /// </summary>
        RightWindows = 92,

        /// <summary>
        /// The application key or button.
        /// </summary>
        Application = 93,

        /// <summary>
        /// The sleep key or button.
        /// </summary>
        Sleep = 95,

        /// <summary>
        /// The number "0" key as located on a numeric pad.
        /// </summary>
        NumberPad0 = 96,

        /// <summary>
        /// The number "1" key as located on a numeric pad.
        /// </summary>
        NumberPad1 = 97,

        /// <summary>
        /// The number "2" key as located on a numeric pad.
        /// </summary>
        NumberPad2 = 98,

        /// <summary>
        /// The number "3" key as located on a numeric pad.
        /// </summary>
        NumberPad3 = 99,

        /// <summary>
        /// The number "4" key as located on a numeric pad.
        /// </summary>
        NumberPad4 = 100,

        /// <summary>
        /// The number "5" key as located on a numeric pad.
        /// </summary>
        NumberPad5 = 101,

        /// <summary>
        /// The number "6" key as located on a numeric pad.
        /// </summary>
        NumberPad6 = 102,

        /// <summary>
        /// The number "7" key as located on a numeric pad.
        /// </summary>
        NumberPad7 = 103,

        /// <summary>
        /// The number "8" key as located on a numeric pad.
        /// </summary>
        NumberPad8 = 104,

        /// <summary>
        /// The number "9" key as located on a numeric pad.
        /// </summary>
        NumberPad9 = 105,

        /// <summary>
        /// The multiply (*) operation key as located on a numeric pad.
        /// </summary>
        Multiply = 106,

        /// <summary>
        /// The add (+) operation key as located on a numeric pad.
        /// </summary>
        Add = 107,

        /// <summary>
        /// The separator key as located on a numeric pad.
        /// </summary>
        Separator = 108,

        /// <summary>
        /// The subtract (-) operation key as located on a numeric pad.
        /// </summary>
        Subtract = 109,

        /// <summary>
        /// The decimal (.) key as located on a numeric pad.
        /// </summary>
        Decimal = 110,

        /// <summary>
        /// The divide (/) operation key as located on a numeric pad.
        /// </summary>
        Divide = 111,

        /// <summary>
        /// The F1 function key.
        /// </summary>
        F1 = 112,

        /// <summary>
        /// The F2 function key.
        /// </summary>
        F2 = 113,

        /// <summary>
        /// The F3 function key.
        /// </summary>
        F3 = 114,

        /// <summary>
        /// The F4 function key.
        /// </summary>
        F4 = 115,

        /// <summary>
        /// The F5 function key.
        /// </summary>
        F5 = 116,

        /// <summary>
        /// The F6 function key.
        /// </summary>
        F6 = 117,

        /// <summary>
        /// The F7 function key.
        /// </summary>
        F7 = 118,

        /// <summary>
        /// The F8 function key.
        /// </summary>
        F8 = 119,

        /// <summary>
        /// The F9 function key.
        /// </summary>
        F9 = 120,

        /// <summary>
        /// The F10 function key.
        /// </summary>
        F10 = 121,

        /// <summary>
        /// The F11 function key.
        /// </summary>
        F11 = 122,

        /// <summary>
        /// The F12 function key.
        /// </summary>
        F12 = 123,

        /// <summary>
        /// The F13 function key.
        /// </summary>
        F13 = 124,

        /// <summary>
        /// The F14 function key.
        /// </summary>
        F14 = 125,

        /// <summary>
        /// The F15 function key.
        /// </summary>
        F15 = 126,

        /// <summary>
        /// The F16 function key.
        /// </summary>
        F16 = 0x7F,

        /// <summary>
        /// The F17 function key.
        /// </summary>
        F17 = 0x80,

        /// <summary>
        /// The F18 function key.
        /// </summary>
        F18 = 129,

        /// <summary>
        /// The F19 function key.
        /// </summary>
        F19 = 130,

        /// <summary>
        /// The F20 function key.
        /// </summary>
        F20 = 131,

        /// <summary>
        /// The F21 function key.
        /// </summary>
        F21 = 132,

        /// <summary>
        /// The F22 function key.
        /// </summary>
        F22 = 133,

        /// <summary>
        /// The F23 function key.
        /// </summary>
        F23 = 134,

        /// <summary>
        /// The F24 function key.
        /// </summary>
        F24 = 135,

        /// <summary>
        /// The navigation up button.
        /// </summary>
        NavigationView = 136,


        /// <summary>
        /// The navigation menu button.
        /// </summary>
        NavigationMenu = 137,

        /// <summary>
        /// The navigation up button.
        /// </summary>
        NavigationUp = 138,

        /// <summary>
        /// The navigation down button.
        /// </summary>
        NavigationDown = 139,

        /// <summary>
        /// The navigation left button.
        /// </summary>
        NavigationLeft = 140,

        /// <summary>
        /// The navigation right button.
        /// </summary>
        NavigationRight = 141,

        /// <summary>
        /// The navigation accept button.
        /// </summary>
        NavigationAccept = 142,

        /// <summary>
        /// The navigation cancel button.
        /// </summary>
        NavigationCancel = 143,

        /// <summary>
        /// The Num Lock key.
        /// </summary>
        NumberKeyLock = 144,

        /// <summary>
        /// The Scroll Lock (ScrLk) key.
        /// </summary>
        Scroll = 145,

        /// <summary>
        /// The left Shift key.
        /// </summary>
        LeftShift = 160,

        /// <summary>
        /// The right Shift key.
        /// </summary>
        RightShift = 161,

        /// <summary>
        /// The left Ctrl key.
        /// </summary>
        LeftControl = 162,

        /// <summary>
        /// The right Ctrl key.
        /// </summary>
        RightControl = 163,

        /// <summary>
        /// The left menu key.
        /// </summary>
        LeftMenu = 164,

        /// <summary>
        /// The right menu key.
        /// </summary>
        RightMenu = 165,

        /// <summary>
        /// The go back key.
        /// </summary>
        GoBack = 166,

        /// <summary>
        /// The go forward key.
        /// </summary>
        GoForward = 167,

        /// <summary>
        /// The refresh key.
        /// </summary>
        Refresh = 168,

        /// <summary>
        /// The stop key.
        /// </summary>
        Stop = 169,

        /// <summary>
        /// The search key.
        /// </summary>
        Search = 170,

        /// <summary>
        /// The favorites key.
        /// </summary>
        Favorites = 171,

        /// <summary>
        /// The go home key.
        /// </summary>
        GoHome = 172,

        /// <summary>
        /// The gamepad A button.
        /// </summary>
        GamepadA = 195,

        /// <summary>
        /// The gamepad B button.
        /// </summary>
        GamepadB = 196,

        /// <summary>
        /// The gamepad X button.
        /// </summary>
        GamepadX = 197,

        /// <summary>
        /// The gamepad Y button.
        /// </summary>
        GamepadY = 198,

        /// <summary>
        /// The gamepad right shoulder.
        /// </summary>
        GamepadRightShoulder = 199,

        /// <summary>
        /// The gamepad left shoulder.
        /// </summary>
        GamepadLeftShoulder = 200,

        /// <summary>
        /// The gamepad left trigger.
        /// </summary>
        GamepadLeftTrigger = 201,

        /// <summary>
        /// The gamepad right trigger.
        /// </summary>
        GamepadRightTrigger = 202,

        /// <summary>
        /// The gamepad d-pad up.
        /// </summary>
        GamepadDPadUp = 203,

        /// <summary>
        /// The gamepad d-pad down.
        /// </summary>
        GamepadDPadDown = 204,

        /// <summary>
        /// The gamepad d-pad left.
        /// </summary>
        GamepadDPadLeft = 205,

        /// <summary>
        /// The gamepad d-pad right.
        /// </summary>
        GamepadDPadRight = 206,

        /// <summary>
        /// The gamepad menu button.
        /// </summary>
        GamepadMenu = 207,

        /// <summary>
        /// The gamepad view button.
        /// </summary>
        GamepadView = 208,

        /// <summary>
        /// The gamepad left thumbstick button.
        /// </summary>
        GamepadLeftThumbstickButton = 209,

        /// <summary>
        /// The gamepad right thumbstick button.
        /// </summary>
        GamepadRightThumbstickButton = 210,

        /// <summary>
        /// The gamepad left thumbstick up.
        /// </summary>
        GamepadLeftThumbstickUp = 211,

        /// <summary>
        /// The gamepad left thumbstick down.
        /// </summary>
        GamepadLeftThumbstickDown = 212,

        /// <summary>
        /// The gamepad left thumbstick right.
        /// </summary>
        GamepadLeftThumbstickRight = 213,

        /// <summary>
        /// The gamepad left thumbstick left.
        /// </summary>
        GamepadLeftThumbstickLeft = 214,

        /// <summary>
        /// The gamepad right thumbstick up.
        /// </summary>
        GamepadRightThumbstickUp = 215,

        /// <summary>
        /// The gamepad right thumbstick down.
        /// </summary>
        GamepadRightThumbstickDown = 216,

        /// <summary>
        /// The gamepad right thumbstick right.
        /// </summary>
        GamepadRightThumbstickRight = 217,

        /// <summary>
        /// The gamepad right thumbstick left.
        /// </summary>
        GamepadRightThumbstickLeft = 218
    }
}
