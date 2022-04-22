// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Services.AppConnections;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace Quarrel
{
    partial class App
    {
        private void SetupAppServiceConnection(IBackgroundTaskInstance taskInstance)
        {
            AppConnectionService? appConnectionService = Services.GetService<AppConnectionService>();
            if (appConnectionService is not null)
            {
                appConnectionService.RegisterAppConnection(taskInstance);
            }
        }
    }
}
