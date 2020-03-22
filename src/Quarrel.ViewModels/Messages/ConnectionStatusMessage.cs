// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Messages
{
    /// <summary>
    /// The connection status of the app.
    /// </summary>
    public enum ConnectionStatus
    {
        Starting,
        Connecting,
        Connected,
        Disconnected,
        Failed,
        Offline,
    }

    /// <summary>
    /// A message that indicates the connection status changed.
    /// </summary>
    public class ConnectionStatusMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStatusMessage"/> class.
        /// </summary>
        /// <param name="status">The new status.</param>
        public ConnectionStatusMessage(ConnectionStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Gets the apps connection status.
        /// </summary>
        public ConnectionStatus Status { get; }
    }
}
