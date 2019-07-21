using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;

namespace Quarrel.Navigation
{
    public interface ISubFrameNavigationService : INavigationService
    {
        object Parameter { get; }
    }
}
