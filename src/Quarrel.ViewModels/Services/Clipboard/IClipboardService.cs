// Quarrel © 2022

using System;

namespace Quarrel.Services.Clipboard
{
    /// <summary>
    /// An interface for a service that handles interactions with the clipboard.
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Copies text to the clipboard.
        /// </summary>
        /// <param name="text">The text to copy.</param>
        /// <param name="flush">Whether or not to flush the clipboard for persistent when the app closes.</param>
        void Copy(string text, bool flush = true);

        /// <summary>
        /// Copies text to the clipboard.
        /// </summary>
        /// <param name="uri">The uri to copy.</param>
        /// <param name="flush">Whether or not to flush the clipboard for persistent when the app closes.</param>
        void Copy(Uri uri, bool flush = true);
    }
}
