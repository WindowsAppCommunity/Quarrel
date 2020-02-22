// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Services.Clipboard;
using Windows.ApplicationModel.DataTransfer;

namespace Quarrel.Services.Clipboard
{
    /// <summary>
    /// A <see langword="class"/> that provides easy access to the windows clipboard.
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        /// <summary>
        /// Sets <paramref name="text"/> as clipboard.
        /// </summary>
        /// <param name="text">String to set as clipboard.</param>
        public void CopyToClipboard(string text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }
    }
}
