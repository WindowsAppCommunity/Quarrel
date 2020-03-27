// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Helpers
{
    /// <summary>
    /// Constants for the App.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// A <see langword="class"/> with some commonly used values for the cache services.
        /// </summary>
        public static class Cache
        {
            /// <summary>
            /// A <see langword="class"/> with some commonly used keys for cached items.
            /// </summary>
            /// <remarks>Should be depricated.</remarks>
            public static class Keys
            {
                /// <summary>
                /// A key for the Login access token.
                /// </summary>
                public const string AccessToken = nameof(AccessToken);

                /// <summary>
                /// A key for GuildRoles.
                /// </summary>
                public const string GuildRole = nameof(GuildRole);

                /// <summary>
                /// A key for Notes.
                /// </summary>
                public const string Note = nameof(Note);
            }
        }

        /// <summary>
        /// Types of Connected animations.
        /// </summary>
        public static class ConnectedAnimationKeys
        {
            /// <summary>
            /// Member flyout Connected Animation key.
            /// </summary>
            public const string MemberFlyoutAnimation = nameof(MemberFlyoutAnimation);
        }

        /// <summary>
        /// Types of commands over the ConnectionService api.
        /// </summary>
        public static class ConnectionServiceRequests
        {
            /// <summary>
            /// Key for a Set Activity command.
            /// </summary>
            public const string SetActivity = "SET_ACTIVITY";
        }

        /// <summary>
        /// Regular Expressions used for the app.
        /// </summary>
        public static class Regex
        {
            /// <summary>
            /// A Regular Expression for finding a user mention.
            /// </summary>
            public const string UserMentionSurrogateRegex = @"@(\w+)#([0-9]{4})";

            /// <summary>
            /// A Regular Expression for finding a channel mention.
            /// </summary>
            public const string ChannelMentionSurrogateRegex = @"#((?:[A-Z]|[a-z]|\-)+)";

            /// <summary>
            /// A Regular Expression for finding an emoji surrogate.
            /// </summary>
            public const string EmojiSurrogateRegex = @"\:([^\s]+)\:";

            /// <summary>
            /// A Regular Expression for finding spotify assets.
            /// </summary>
            public const string SpotifyLargeAssetsIdRegex = @"spotify:(\w+)";

            /// <summary>
            /// A Regular Expression for finding a YouTube link.
            /// </summary>
            public const string YouTubeURLRegex = @"(?:https:\/\/)?(?:(?:www\.)?youtube\.com\/(?:(?:watch\?.*?v=)|(?:embed\/))([\w\-]+)|youtu\.be\/(?:embed\/)?([\w\-]+))";
        }

        /// <summary>
        /// Application Id constants.
        /// </summary>
        public static class Store
        {
            /// <summary>
            /// The GitHub repository owner.
            /// </summary>
            public const string GitHubRepoOwner = "UWPCommunity";

            /// <summary>
            /// The GitHub repository name.
            /// </summary>
            public const string GitHubRepoName = "Quarrel";
        }

        /// <summary>
        /// A <see langword="class"/> with some commonly used values for the analytics service.
        /// </summary>
        public static class Analytics
        {
            /// <summary>
            /// A <see langword="class"/> with the collection of tracked events.
            /// </summary>
            public static class Events
            {
#pragma warning disable SA1600 // Elements should be documented
                public const string ReadyPacketRecieved = "Ready packet recieved";
                public const string ConnectionAttempt = "Connection attempted";
                public const string Connected = "Connected";
                public const string Disconnected = "Disconnected";
                public const string InvalidSession = "Invalid session";
#pragma warning restore SA1600 // Elements should be documented
            }
        }
    }
}
