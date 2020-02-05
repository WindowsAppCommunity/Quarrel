using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;

namespace Quarrel.ViewModels.Services.Navigation
{
    public interface ISubFrameNavigationService : INavigationService
    {
        /// <summary>
        /// Extra parameter for navigation
        /// </summary>
        object Parameter { get; }

        /// <summary>
        /// How many levels of subpages deep the user is
        /// </summary>
        int Depth { get; }
    }
}
