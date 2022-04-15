// Quarrel © 2022

using Quarrel.Services.Analytics.Enums;
using System.Diagnostics;

namespace Quarrel.Services.Analytics
{
    public class LoggingAnalyticsService : IAnalyticsService
    {
        /// <inheritdoc/>
        public void Log(string title, params (string, object)[] data)
        {
            Debug.WriteLine($"Event: {title}");
            foreach ((string item1, object item2) in data)
            {
                Debug.WriteLine($"{item1}: {item2}");
            }
        }

        /// <inheritdoc/>
        public void Log(LoggedEvent eventType, params (string, object)[] data)
        {
            Log($"{eventType}", data);
        }
    }
}
