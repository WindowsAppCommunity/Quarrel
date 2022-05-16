// Quarrel © 2022

using System;
using Windows.ApplicationModel.DataTransfer;
using Clip = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace Quarrel.Services.Clipboard
{
    public class ClipboardService : IClipboardService
    {
        public void Copy(string text, bool flush = true)
        {
            DataPackage package = new DataPackage();
            package.SetText(text);
            Copy(package, flush);
        }

        public void Copy(Uri uri, bool flush = true)
        {
            DataPackage package = new DataPackage();
            package.SetWebLink(uri);
            Copy(package, flush);
        }

        public void Copy(DataPackage data, bool flush = true)
        {
            data.RequestedOperation = DataPackageOperation.Copy;
            Clip.SetContent(data);
            if (flush) Clip.Flush();
        }
    }
}
