// Quarrel © 2022

using Quarrel.Attributes;

namespace Quarrel.Services.Analytics.Enums
{
    /// <summary>
    /// An enum for event types to log.
    /// </summary>
    public enum LoggedEvent
    {
        #region App
        /// <summary>
        /// A subpage opened.
        /// </summary>
        [StringValue("SubPage Opened")]
        SubPageOpened,

        /// <summary>
        /// A message was marked read.
        /// </summary>
        [StringValue("Message Marked as Read")]
        MarkRead,

        /// <summary>
        /// A message was sent.
        /// </summary>
        [StringValue("Message Sent")]
        MessageSent,

        /// <summary>
        /// A message was deleted.
        /// </summary>
        [StringValue("Message Deleted")]
        MessageDeleted,

        /// <summary>
        /// A class was started.
        /// </summary>
        [StringValue("Call Started")]
        CallStarted,

        /// <summary>
        /// The user's status was set.
        /// </summary>
        [StringValue("Status Set")]
        StatusSet,

        /// <summary>
        /// Opened a channel.
        /// </summary>
        [StringValue("Guild Opened")]
        GuildOpened,

        /// <summary>
        /// Opened a channel.
        /// </summary>
        [StringValue("Channel Opened")]
        ChannelOpened,
        #endregion

        #region Login
        /// <summary>
        /// The app attempted to login.
        /// </summary>
        [StringValue("Login Attempted")]
        LoginAttempted,

        /// <summary>
        /// The app logged in successfully.
        /// </summary>
        [StringValue("Successful Login")]
        SuccessfulLogin,

        /// <summary>
        /// The app logged failed to login.
        /// </summary>
        [StringValue("Login Failed")]
        LoginFailed,
        #endregion

        #region API

        /// <summary>
        /// Encountered an issue in an http request, but it was handled.
        /// </summary>
        [StringValue("Http Exception Handled")]
        HttpExceptionHandled,

        #endregion

        #region Gateway
        /// <summary>
        /// The gateway encountered a known event.
        /// </summary>
        [StringValue("Known Gateway Event Encountered")]
        KnownGatewayEventEncountered,

        /// <summary>
        /// The gateway encountered an issue, but it was handled.
        /// </summary>
        [StringValue("Gateway Exception Handled")]
        GatewayExceptionHandled,

        /// <summary>
        /// The gateway encountered a unknown operation.
        /// </summary>
        [StringValue("Unknown Gateway Operation Encountered")]
        UnknownGatewayOperationEncountered,

        /// <summary>
        /// The gateway encountered a unknown event.
        /// </summary>
        [StringValue("Unknown Gateway Event Encountered")]
        UnknownGatewayEventEncountered,

        /// <summary>
        /// The gateway encountered a known operation but does not handle it.
        /// </summary>
        [StringValue("Unhandled Gateway Operation Encountered")]
        UnhandledGatewayOperationEncountered,

        /// <summary>
        /// The gateway encountered a known event but does not handle it.
        /// </summary>
        [StringValue("Unhandled Gateway Event Encountered")]
        UnhandledGatewayEventEncountered,
        #endregion

        #region Other APIs

        /// <summary>
        /// The patreon client info could not be found.
        /// </summary>
        [StringValue("Patreon ClientInfo Not Found")]
        PatreonClientInfoNotFound,

        /// <summary>
        /// A request to the GitHub API failed.
        /// </summary>
        [StringValue("GitHub Request Failed")]
        GitHubRequestFailed,

        /// <summary>
        /// A request to the Discord Status API failed.
        /// </summary>
        [StringValue("Discord Status Request Failed")]
        DiscordStatusRequestFailed,

        #endregion

        #region AppServiceConnection

        /// <summary>
        /// An app service connection was actived.
        /// </summary>
        [StringValue("App Service Connection Received")]
        AppServiceConnectionReceived,

        #endregion
    }
}
