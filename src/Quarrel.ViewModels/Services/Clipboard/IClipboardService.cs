// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Clipboard
{
    /// <summary>
    /// A service for interacting with the clipboard.
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Copies a string to the clipboard.
        /// </summary>
        /// <param name="text">New clipboard.</param>
        void CopyToClipboard(string text);
    }
}
