// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Abstract
{
    public abstract class BindableItem : ObservableObject
    {
        protected readonly IDispatcherService _dispatcherService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableItem"/> class.
        /// </summary>
        public BindableItem(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
    }
}
