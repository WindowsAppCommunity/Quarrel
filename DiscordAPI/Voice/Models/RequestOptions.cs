using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Discord_UWP.Voice
{
    public class RequestOptions
    {
        public static RequestOptions Default => new RequestOptions();

        /// <summary> 
        /// The max time, in milliseconds, to wait for this request to complete. If null, a request will not time out. 
        /// If a rate limit has been triggered for this request's bucket and will not be unpaused in time, this request will fail immediately. 
        /// </summary>
        public int? Timeout { get; set; }
        public CancellationToken CancelToken { get; set; } = CancellationToken.None;
        public RetryMode? RetryMode { get; set; }
        public bool HeaderOnly { get; internal set; }
        /// <summary>
        /// The reason for this action in the guild's audit log
        /// </summary>
        public string AuditLogReason { get; set; }

        internal bool IgnoreState { get; set; }
        internal string BucketId { get; set; }
        internal bool IsClientBucket { get; set; }

        internal static RequestOptions CreateOrClone(RequestOptions options)
        {
            if (options == null)
                return new RequestOptions();
            else
                return options.Clone();
        }

        public RequestOptions()
        {
            Timeout = DiscordConfig.DefaultRequestTimeout;
        }

        public RequestOptions Clone() => MemberwiseClone() as RequestOptions;
    }

    [Flags]
    public enum RetryMode
    {
        /// <summary> If a request fails, an exception is thrown immediately. </summary>
        AlwaysFail = 0x0,
        /// <summary> Retry if a request timed out. </summary>
        RetryTimeouts = 0x1,
        // /// <summary> Retry if a request failed due to a network error. </summary>
        //RetryErrors = 0x2,
        /// <summary> Retry if a request failed due to a ratelimit. </summary>
        RetryRatelimit = 0x4,
        /// <summary> Retry if a request failed due to an HTTP error 502. </summary>
        Retry502 = 0x8,
        /// <summary> Continuously retry a request until it times out, its cancel token is triggered, or the server responds with a non-502 error. </summary>
        AlwaysRetry = RetryTimeouts | /*RetryErrors |*/ RetryRatelimit | Retry502,
    }

    public class DiscordConfig
    {
        public const int APIVersion = 6;
        public static string Version { get; } =
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(DiscordConfig).GetTypeInfo().Assembly.GetName().Version.ToString(3) ??
            "Unknown";

        public static string UserAgent { get; } = $"DiscordBot (https://github.com/RogueException/Discord.Net, v{Version})";
        public static readonly string APIUrl = $"https://discordapp.com/api/v{APIVersion}/";
        public const string CDNUrl = "https://cdn.discordapp.com/";
        public const string InviteUrl = "https://discord.gg/";

        public const int DefaultRequestTimeout = 15000;
        public const int MaxMessageSize = 2000;
        public const int MaxMessagesPerBatch = 100;
        public const int MaxUsersPerBatch = 1000;
        public const int MaxGuildsPerBatch = 100;

        /// <summary> Gets or sets how a request should act in the case of an error, by default. </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;

        /// <summary> Gets or sets the minimum log level severity that will be sent to the Log event. </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        /// <summary> Gets or sets whether the initial log entry should be printed. </summary>
        internal bool DisplayInitialLog { get; set; } = true;
    }

    public enum LogSeverity
    {
        Critical = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Verbose = 4,
        Debug = 5
    }
}
