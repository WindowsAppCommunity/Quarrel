using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.DispatcherHelper
{
    public interface IDispatcherHelper
    {
        void CheckBeginInvokeOnUi(Action action);
    }
}
