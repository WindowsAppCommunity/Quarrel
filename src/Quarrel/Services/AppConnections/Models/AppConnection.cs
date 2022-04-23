// Quarrel © 2022

using Windows.ApplicationModel.AppService;

namespace Quarrel.Services.AppConnections.Models
{
    public abstract class AppConnection
    {
        private readonly AppServiceConnection _appServiceConnection;

        public AppConnection(AppServiceConnection appServiceConnection)
        {
            _appServiceConnection = appServiceConnection;
            _appServiceConnection.RequestReceived += OnRequestReceived;
            _appServiceConnection.ServiceClosed += OnServiceClosed;
        }

        protected virtual void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args) { }

        protected virtual void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args) { }
    }
}
