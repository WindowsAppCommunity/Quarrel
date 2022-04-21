// Quarrel © 2022

using Quarrel.RichPresence.Models;
using Quarrel.RichPresence.Models.Enums;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Quarrel.RichPresence
{
    public class RichPresenceConnection
    {
        private readonly AppServiceConnection _connection;

        /// <summary>
        /// Fired when the connection to the app service closes.
        /// </summary>
        public event EventHandler ConnectionClosed;

        public AppServiceConnectionStatus? ConnectionStatus { get; private set; }

        public async Task<bool> ConnectAsync(AppVersionType versionType = AppVersionType.Release)
        {
            if (ConnectionStatus == AppServiceConnectionStatus.Success)
                return true;

            _connection.AppServiceName = AppConnectionInfo.RichPresenceServiceName;
            _connection.PackageFamilyName = AppConnectionInfo.GetPackageFamilyName(versionType);
            _connection.ServiceClosed += OnClosed;

            ConnectionStatus = await _connection.OpenAsync();
            return ConnectionStatus == AppServiceConnectionStatus.Success;
        }
        
        public async Task<bool> SetActivity(Activity activity)
        {
            ValueSet request = FormRequest(RequestType.SetActivity);

            var response = await _connection.SendMessageAsync(request);
            return response.Status == AppServiceResponseStatus.Success;
        }

        private ValueSet FormRequest(RequestType type, params (string, object)[] data)
        {
            ValueSet request = new ValueSet();
            request.Add("type", type);

            foreach ((string property, object value) in data)
            {
                request.Add(property, value);
            }

            return request;
        }

        private void OnClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            ConnectionStatus = null;
            ConnectionClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
