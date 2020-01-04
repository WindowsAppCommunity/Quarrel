using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Clipboard
{
    public interface IClipboardService
    {
        public void CopyToClipboard(string text);
    }
}
