// Quarrel © 2022

using Quarrel.Client.Logger;
using Quarrel.Services.Analytics.Enums;

namespace Quarrel.Services.Analytics
{
    /// <summary>
    /// An interface for a service that logs events.
    /// </summary>
    public interface ILoggingService : IClientLogger
    {
        /// <summary>
        /// Logs an event with a specified title and optional properties.
        /// </summary>
        /// <param name="eventType">The type of event to track.</param>
        /// <param name="data">The optional event properties.</param>
        void Log(LoggedEvent eventType, params (string property, string value)[] data);
    }
}
