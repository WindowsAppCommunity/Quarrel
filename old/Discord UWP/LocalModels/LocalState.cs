using System.Collections.Generic;
using Windows.UI.Xaml;
using DiscordAPI.API.Game;
using DiscordAPI.SharedModels;

namespace Quarrel.LocalModels
{
    public static class LocalState
    {
        /// <summary>
        /// Dictionary of Guilds by Id
        /// </summary>
        public static Dictionary<string, Guild> Guilds = new Dictionary<string, Guild>();

        /// <summary>
        /// Dictionary of DM Channels by Id
        /// </summary>
        public static Dictionary<string, DirectMessageChannel> DMs = new Dictionary<string, DirectMessageChannel>();

        /// <summary>
        /// API object for current user
        /// </summary>
        public static User CurrentUser = new User();

        /// <summary>
        /// Dictionary of Friends by UserId
        /// </summary>
        public static Dictionary<string, Friend> Friends = new Dictionary<string, Friend>();
        
        /// <summary>
        /// Dictionary of blocked users by UserId
        /// </summary>
        public static Dictionary<string, Friend> Blocked = new Dictionary<string, Friend>();

        /// <summary>
        /// Dictionary of user notes by UserId
        /// </summary>
        public static Dictionary<string, string> Notes = new Dictionary<string, string>();

        /// <summary>
        /// API object of User Settings
        /// </summary>
        public static UserSettings Settings = new UserSettings();

        /// <summary>
        /// Dictionary of Dictionary of Timers by UserId, by ChannelId
        /// </summary>
        public static Dictionary<string, Dictionary<string, DispatcherTimer>> Typers = new Dictionary<string, Dictionary<string, DispatcherTimer>>();

        /// <summary>
        /// List of Typing timers
        /// </summary>
        public static List<DispatcherTimer> TyperTimers = new List<DispatcherTimer>();

        /// <summary>
        /// Dictionary of User Presence by UserId
        /// </summary>
        public static Dictionary<string, Presence> PresenceDict = new Dictionary<string, Presence>();

        /// <summary>
        /// Dictionary of User Voice State by UserId
        /// </summary>
        public static Dictionary<string, VoiceState> VoiceDict = new Dictionary<string, VoiceState>();

        /// <summary>
        /// Dictionary of Channel read state by ChannelId
        /// </summary>
        public static Dictionary<string, ReadState> RPC = new Dictionary<string, ReadState>();

        /// <summary>
        /// Dictionary of GuildSettings by GuildId
        /// </summary>
        public static Dictionary<string, GuildSetting> GuildSettings = new Dictionary<string, GuildSetting>();

        /// <summary>
        /// Current User Voice State
        /// </summary>
        public static VoiceState VoiceState = new VoiceState();

        /// <summary>
        /// Dictionary of Supported Games by GameId
        /// </summary>
        public static Dictionary<string, GameListItem> SupportedGames = new Dictionary<string, GameListItem>();

        /// <summary>
        /// Dictionary of Support Games' Names by GameId
        /// </summary>
        public static Dictionary<string, string> SupportedGamesNames = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary of drafts by ChannelId
        /// </summary>
        public static Dictionary<string, string> Drafts = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary of List of Game News by GameId
        /// </summary>
        public static Dictionary<string, List<GameNews>> GameNews = new Dictionary<string, List<GameNews>>();

        /// <summary>
        /// User Feed Settings
        /// </summary>
        public static FeedSettings FeedSettings = new FeedSettings();

        /// <summary>
        /// Currently Selected Guild
        /// </summary>
        public static Guild CurrentGuild => App.CurrentGuildId != null && Guilds.ContainsKey(App.CurrentGuildId) ? Guilds[App.CurrentGuildId] : null;

        /// <summary>
        /// Currently Selected Guild Channel
        /// </summary>
        public static GuildChannel CurrentGuildChannel => CurrentGuild != null && CurrentGuild.channels.ContainsKey(App.CurrentChannelId) ? CurrentGuild.channels[App.CurrentChannelId] : null;

        /// <summary>
        /// Currently Selected DM Channel
        /// </summary>
        public static DirectMessageChannel CurrentDMChannel => App.CurrentChannelId != null && DMs.ContainsKey(App.CurrentChannelId) ? DMs[App.CurrentChannelId] : null;

        /// <summary>
        /// Current User Presence
        /// </summary>
        public static Presence CurrentUserPresence => PresenceDict.ContainsKey(LocalState.CurrentUser.Id) ? PresenceDict[LocalState.CurrentUser.Id] : null;
    }
}
