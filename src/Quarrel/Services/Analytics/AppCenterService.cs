// Quarrel © 2022

#if !DEV

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Quarrel.Services.Analytics.Enums;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;
using System.Reflection;
using System.Text.Json;

namespace Quarrel.Services.Analytics
{
    public class AppCenterService : IAnalyticsService
    {
        #if RELEASE
        private const string ClientInfoFile = "AppCenter.json";
        #elif INSIDER
        private const string ClientInfoFile = "AppCenterInsider.json";
        #elif ALPHA
        private const string ClientInfoFile = "AppCenterAlpha.json";
        #endif
        private const string ClientInfoDirectory = "Assets/Tokens/AppCenter";
        private const string ClientInfoPath = $"{ClientInfoDirectory}/{ClientInfoFile}";

        private const int PropertyValueMaxLength = 125;
        private readonly AppCenterClientInfo _clientInfo;

        public AppCenterService()
        {
            string json = Assembly.GetExecutingAssembly().ReadEmbeddedFile(ClientInfoFile);
            _clientInfo = JsonSerializer.Deserialize<AppCenterClientInfo>(json);

            AppCenter.Start(_clientInfo.Secret,
                typeof(AppCenterAnalytics),
                typeof(Crashes));
        }

        public void Log(LoggedEvent eventType, params (string property, string value)[] data)
        {
            IDictionary<string, string> properties = data?.ToDictionary(
                pair => pair.property,
                pair => pair.value.Length <= PropertyValueMaxLength ? pair.value : pair.value.Substring(0, PropertyValueMaxLength));

            AppCenterAnalytics.TrackEvent($"{eventType.GetStringValue()}", properties);
        }
    }
}

#endif