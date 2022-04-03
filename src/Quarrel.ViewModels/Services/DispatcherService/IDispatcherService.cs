// Adam Dernis © 2022

using System;

namespace Quarrel.Services.DispatcherService
{
    public interface IDispatcherService
    {
        void RunOnUIThread(Action action);
    }
}
