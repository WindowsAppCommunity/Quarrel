// Quarrel © 2022

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Quarrel.Client.Logger;
using Quarrel.Services.Analytics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace Quarrel.Services.Analytics
{
    public class AppCenterService : ILoggingService
    {
#if RELEASE
        private const string ClientInfoFile = "AppCenter.json";
#elif INSIDER
        private const string ClientInfoFile = "AppCenterInsider.json";
#elif ALPHA
        private const string ClientInfoFile = "AppCenterAlpha.json";
#endif
#if !DEV
        private const string ClientInfoDirectory = "Assets/Tokens/AppCenter";
        private const string ClientInfoPath = $"{ClientInfoDirectory}/{ClientInfoFile}";
#endif

        private const int PropertyValueMaxLength = 125;
        private readonly AppCenterClientInfo? _clientInfo;

        public AppCenterService()
        {
            string json = string.Empty;
#if !DEV
            json = Assembly.GetExecutingAssembly().ReadEmbeddedFile(ClientInfoFile);
#endif
            _clientInfo = JsonSerializer.Deserialize<AppCenterClientInfo>(json);

            AppCenter.Start(_clientInfo.Secret,
                typeof(AppCenterAnalytics),
                typeof(Crashes));
        }

        public void Log(LoggedEvent eventType, params (string property, string value)[] data) =>
            Log(eventType.GetStringValue(), data);

        public void Log(ClientLogEvent eventType, params (string property, string value)[] data) =>
            Log(eventType.GetStringValue(), data);

        private static void Log(string eventType, params (string property, string value)[] data)
        {
            IDictionary<string, string>? properties = null;
            if (data.Length != 0)
            {
                properties = data.ToDictionary(
                    pair => pair.property,
                    pair => pair.value.Length <= PropertyValueMaxLength ? pair.value : pair.value.Substring(0, PropertyValueMaxLength));
            }

            AppCenterAnalytics.TrackEvent($"{eventType}", properties);
        }
    }
}
