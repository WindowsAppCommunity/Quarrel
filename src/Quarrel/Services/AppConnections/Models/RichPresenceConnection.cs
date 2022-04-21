// Quarrel © 2022

using Windows.ApplicationModel.AppService;

namespace Quarrel.Services.AppConnections.Models
{
    public class RichPresenceConnection : AppConnection
    {
        public RichPresenceConnection(AppServiceConnection appServiceConnection) :
            base(appServiceConnection)
        {
        }
    }
}
