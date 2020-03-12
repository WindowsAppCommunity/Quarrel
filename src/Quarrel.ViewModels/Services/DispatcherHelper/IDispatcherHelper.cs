// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.ViewModels.Services.DispatcherHelper
{
    /// <summary>
    /// A Dispatcher to the UI thread.
    /// </summary>
    public interface IDispatcherHelper
    {
        /// <summary>
        /// Runs <paramref name="action"/> on the UI thread.
        /// </summary>
        /// <param name="action">Action to run.</param>
        void CheckBeginInvokeOnUi(Action action);
    }
}
