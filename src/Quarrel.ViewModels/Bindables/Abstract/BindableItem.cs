﻿// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Abstract
{
    /// <summary>
    /// An item that can be bound to the UI and contains an <see cref="IDispatcherService"/>.
    /// </summary>
    public abstract class BindableItem : ObservableObject
    {
        /// <summary>
        /// Gets an <see cref="IDispatcherService"/> that can run code on the UI Thread.
        /// </summary>
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