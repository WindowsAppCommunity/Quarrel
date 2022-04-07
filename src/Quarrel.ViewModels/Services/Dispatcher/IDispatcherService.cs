// Adam Dernis © 2022

using System;

namespace Quarrel.Services.Dispatcher
{
    public interface IDispatcherService
    {
        void RunOnUIThread(Action action);
    }
}
