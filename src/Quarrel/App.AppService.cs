// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Services.AppConnections;
using Windows.ApplicationModel.AppService;

namespace Quarrel
{
    partial class App
    {
        private void SetupAppServiceConnection(AppServiceTriggerDetails triggerDetails)
        {
            AppConnectionService? appConnectionService = Services.GetService<AppConnectionService>();
            if (appConnectionService is not null)
            {
                appConnectionService.RegisterAppConnection(triggerDetails);
            }
        }
    }
}
