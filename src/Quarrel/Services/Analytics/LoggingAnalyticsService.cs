// Quarrel © 2022

using System.Diagnostics;

namespace Quarrel.Services.Analytics
{
    public class LoggingAnalyticsService : IAnalyticsService
    {
        public void Log(string title, params (string, string)[] data)
        {
            Debug.WriteLine($"Event: {title}");
        }
    }
}
