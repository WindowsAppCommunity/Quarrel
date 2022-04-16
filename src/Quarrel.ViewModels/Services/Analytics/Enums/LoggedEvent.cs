// Quarrel © 2022

namespace Quarrel.Services.Analytics.Enums
{
    /// <summary>
    /// An enum for event types to log.
    /// </summary>
    public enum LoggedEvent
    {
        #region Login
        /// <summary>
        /// The app logged in successfully.
        /// </summary>
        SuccessfulLogin,

        /// <summary>
        /// The app logged failed to login.
        /// </summary>
        LoginFailed,
        #endregion

        /// <summary>
        /// Encountered an issue in an http request, but it was handled.
        /// </summary>
        HttpExceptionHandled,

        #region Gateway
        /// <summary>
        /// The gateway encountered a known event.
        /// </summary>
        KnownGatewayEventEncountered,

        /// <summary>
        /// The gateway encountered an issue, but it was handled.
        /// </summary>
        GatewayExceptionHandled,

        /// <summary>
        /// The gateway encountered a unknown operation.
        /// </summary>
        UnknownGatewayOperationEncountered,

        /// <summary>
        /// The gateway encountered a unknown event.
        /// </summary>
        UnknownGatewayEventEncountered,

        /// <summary>
        /// The gateway encountered a known operation but does not handle it.
        /// </summary>
        UnhandledGatewayOperationEncountered,

        /// <summary>
        /// The gateway encountered a known event but does not handle it.
        /// </summary>
        UnhandledGatewayEventEncountered,
        #endregion

        #region Other APIs

        /// <summary>
        /// A request to the GitHub API failed.
        /// </summary>
        GitHubRequestFailed,

        #endregion
    }
}
