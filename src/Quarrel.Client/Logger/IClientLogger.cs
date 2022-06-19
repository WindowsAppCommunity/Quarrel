// Quarrel © 2022

namespace Quarrel.Client.Logger
{
    /// <summary>
    /// An interface for a service that logs client events.
    /// </summary>
    public interface IClientLogger
    {
        /// <summary>
        /// Logs an event with a specified title and optional properties.
        /// </summary>
        /// <param name="eventType">The type of event to track.</param>
        /// <param name="data">The optional event properties.</param>
        void Log(ClientLogEvent eventType, params (string property, string value)[] data);
    }
}
