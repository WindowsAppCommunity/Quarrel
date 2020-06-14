// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;

namespace Quarrel.ViewModels.Services.Analytics
{
    /// <summary>
    /// An interface for a service that logs analytics.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Logs an event with a specified title and optional properties.
        /// </summary>
        /// <param name="title">The title of the event to track.</param>
        /// <param name="data">The optional event properties.</param>
        void Log(string title, params (string Property, string Value)[] data);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="e">The exception to track.</param>
        void LogError(Exception e);
    }
}
