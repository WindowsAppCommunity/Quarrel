// Quarrel © 2022

using CommunityToolkit.Diagnostics;
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
        private AppServiceConnection? _connection;

        /// <summary>
        /// Fired when the connection to the app service closes.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichPresenceConnection"/> class.
        /// </summary>
        public RichPresenceConnection()
        {
        }

        public AppServiceConnectionStatus? ConnectionStatus { get; private set; }

        public async Task<bool> ConnectAsync(AppVersionType versionType = AppVersionType.Release)
        {
            if (ConnectionStatus == AppServiceConnectionStatus.Success)
                return true;

            _connection = new AppServiceConnection();
            _connection.AppServiceName = AppConnectionInfo.RichPresenceServiceName;
            _connection.PackageFamilyName = AppConnectionInfo.GetPackageFamilyName(versionType);
            _connection.ServiceClosed += OnClosed;

            ConnectionStatus = await _connection.OpenAsync();
            return ConnectionStatus == AppServiceConnectionStatus.Success;
        }
        
        public async Task<bool> SetActivity(Activity activity)
        {
            Guard.IsNotNull(_connection, nameof(_connection));

            ValueSet request = FormRequest(RequestType.SetActivity,
                (nameof(activity), activity));

            var response = await _connection.SendMessageAsync(request);
            return response.Status == AppServiceResponseStatus.Success;
        }

        private static ValueSet FormRequest(RequestType type, params (string, object)[] data)
        {
            var request = new ValueSet {{"type", type}};

            foreach ((string property, object value) in data)
            {
                request.Add(property, value);
            }

            return request;
        }

        private void OnClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            ConnectionStatus = null;
            _connection = null;
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
