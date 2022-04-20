// Quarrel © 2022

#if !DEV

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Quarrel.Services.Analytics.Enums;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace Quarrel.Services.Analytics
{
    public class AppCenterService : IAnalyticsService
    {
        private const int PropertyValueMaxLength = 125;

        public AppCenterService()
        {
            AppCenter.Start("",
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