// Quarrel © 2022

using Quarrel.Attributes;

namespace Quarrel.Services.Analytics.Enums
{
    /// <summary>
    /// An enum for events to log.
    /// </summary>
    public enum LoggedEvent
    {
        #region App
        /// <summary>
        /// A sub page opened.
        /// </summary>
        [StringValue("SubPage Opened")]
        SubPageOpened,

        /// <summary>
        /// The user marked a message as read.
        /// </summary>
        [StringValue("Message Marked as Read")]
        MarkRead,

        /// <summary>
        /// The user sent a message.
        /// </summary>
        [StringValue("Message Sent")]
        MessageSent,

        /// <summary>
        /// A message was deleted.
        /// </summary>
        [StringValue("Message Deleted")]
        MessageDeleted,

        /// <summary>
        /// The user started a call.
        /// </summary>
        [StringValue("StartedCall")]
        StartedCall,

        /// <summary>
        /// The user's status was set.
        /// </summary>
        [StringValue("Status Set")]
        StatusSet,

        /// <summary>
        /// The user opened a guild.
        /// </summary>
        [StringValue("Guild Opened")]
        GuildOpened,

        /// <summary>
        /// The user opened a channel.
        /// </summary>
        [StringValue("Channel Opened")]
        ChannelOpened,

        /// <summary>
        /// The user joined a call.
        /// </summary>
        [StringValue("Joined Call")]
        JoinedCall,

        /// <summary>
        /// The user left a call.
        /// </summary>
        [StringValue("Left Call")]
        LeftCall,
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
        /// An app service connection was activated.
        /// </summary>
        [StringValue("App Service Connection Received")]
        AppServiceConnectionReceived,

        #endregion
    }
}
