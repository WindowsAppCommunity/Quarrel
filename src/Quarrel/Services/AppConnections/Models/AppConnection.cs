// Quarrel © 2022

using System;
using Windows.ApplicationModel.AppService;

namespace Quarrel.Services.AppConnections.Models
{
    public abstract class AppConnection
    {
        protected readonly AppServiceConnection _appServiceConnection;

        public AppConnection(AppServiceConnection appServiceConnection)
        {
            Id = $"{Guid.NewGuid()}";
            _appServiceConnection = appServiceConnection;
        }

        public string Id { get; }
    }
}
