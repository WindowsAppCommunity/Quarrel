// Quarrel © 2022

using System;

namespace Quarrel.Services.Dispatcher
{
    /// <summary>
    /// An interface for dispatching operations to the UI thread.
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Runs an action on the UI thread.
        /// </summary>
        /// <param name="action">The action to run on the UI thread.</param>
        void RunOnUIThread(Action action);
    }
}
