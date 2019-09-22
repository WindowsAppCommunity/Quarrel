using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Services.DispatcherHelper;

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
