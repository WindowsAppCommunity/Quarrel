// Quarrel © 2022

using Quarrel.Services.Analytics.Enums;
using System.Diagnostics;

namespace Quarrel.Services.Analytics
{
    public class LoggingAnalyticsService : IAnalyticsService
    {
        /// <inheritdoc/>
        public void Log(LoggedEvent eventType, params (string, string)[] data)
        {
            Debug.WriteLine($"Event: {eventType}");
            foreach ((string property, string value) in data)
            {
                Debug.WriteLine($"{property}: {value}");
            }
        }
    }
}
