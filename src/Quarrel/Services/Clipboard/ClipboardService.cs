using Quarrel.ViewModels.Services.Clipboard;
using Windows.ApplicationModel.DataTransfer;

namespace Quarrel.Services.Clipboard
{
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }
    }
}
