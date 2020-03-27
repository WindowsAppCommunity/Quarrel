// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

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
    }
}
