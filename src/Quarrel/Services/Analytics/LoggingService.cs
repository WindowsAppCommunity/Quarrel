// Quarrel © 2022

using Quarrel.Client.Logger;
using Quarrel.Services.Analytics.Enums;
using System.Diagnostics;

namespace Quarrel.Services.Analytics
{
    public class LoggingService : ILoggingService
    {
        /// <inheritdoc/>
        public void Log(LoggedEvent eventType, params (string, string)[] data) => Log($"{eventType}", data);

        /// <inheritdoc/>
        public void Log(ClientLogEvent eventType, params (string property, string value)[] data) => Log($"{eventType}", data);

        private static void Log(string eventType, params (string, string)[] data)
        {
            Debug.WriteLine($"Event: {eventType}");
            foreach ((string property, string value) in data)
            {
                Debug.WriteLine($"{property}: {value}");
            }
        }
    }
}
