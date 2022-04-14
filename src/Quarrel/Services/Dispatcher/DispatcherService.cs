// Quarrel © 2022

using System;
using Windows.System;

namespace Quarrel.Services.Dispatcher
{
    public class DispatcherService : IDispatcherService
    {
        private DispatcherQueue _dispatcherQueue;

        public DispatcherService()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public void RunOnUIThread(Action action)
        {
            _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => action.Invoke());
        }
    }
}
