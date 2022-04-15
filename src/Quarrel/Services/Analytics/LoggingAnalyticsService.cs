// Quarrel © 2022

using Quarrel.Services.Analytics.Enums;
using System.Diagnostics;

namespace Quarrel.Services.Analytics
{
    public class LoggingAnalyticsService : IAnalyticsService
    {
        /// <inheritdoc/>
        public void Log(string title, params (string, string)[] data)
        {
            Debug.WriteLine($"Event: {title}");
            foreach (var item in data)
            {
                Debug.WriteLine($"{item.Item1}: {item.Item2}");
            }
        }

        /// <inheritdoc/>
        public void Log(LoggedEvent eventType, params (string, string)[] data)
        {
            Log($"{eventType}", data);
        }
    }
}
