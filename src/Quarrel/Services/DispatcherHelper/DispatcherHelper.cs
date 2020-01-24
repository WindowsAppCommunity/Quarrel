using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System;

namespace Quarrel.Services.DispatcherHelperEx
{
    public class DispatcherHelperEx : IDispatcherHelper
    {
        public void CheckBeginInvokeOnUi(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}
