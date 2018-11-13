using System.Collections.Generic;
using Windows.UI.Xaml;
using Discord_UWP.API.Game;
using Discord_UWP.SharedModels;

namespace Discord_UWP.LocalModels
{
    public static class LocalState
    {
        public static Dictionary<string, Guild> Guilds = new Dictionary<string, Guild>();
        public static Dictionary<string, DirectMessageChannel> DMs = new Dictionary<string, DirectMessageChannel>();
        public static User CurrentUser = new User();
        public static Dictionary<string, Friend> Friends = new Dictionary<string, Friend>();
        public static Dictionary<string, Friend> Blocked = new Dictionary<string, Friend>();
        public static Dictionary<string, string> Notes = new Dictionary<string, string>();
        public static UserSettings Settings = new UserSettings();

        public static Dictionary<string, Dictionary<string, DispatcherTimer>> Typers = new Dictionary<string, Dictionary<string, DispatcherTimer>>();
        public static List<DispatcherTimer> TyperTimers = new List<DispatcherTimer>();
        public static Dictionary<string, Presence> PresenceDict = new Dictionary<string, Presence>();
        public static Dictionary<string, VoiceState> VoiceDict = new Dictionary<string, VoiceState>();
        public static Dictionary<string, ReadState> RPC = new Dictionary<string, ReadState>();
        public static Dictionary<string, GuildSetting> GuildSettings = new Dictionary<string, GuildSetting>();
        public static VoiceState VoiceState = new VoiceState();
        public static Dictionary<string, API.Game.GameListItem> SupportedGames = new Dictionary<string, GameListItem>();
        public static Dictionary<string, string> SupportedGamesNames = new Dictionary<string, string>();
        public static Dictionary<string, string> Drafts = new Dictionary<string, string>();
        public static Dictionary<string, List<GameNews>> GameNews = new Dictionary<string, List<GameNews>>();
        public static FeedSettings FeedSettings = new FeedSettings();

        public static Guild CurrentGuild => App.CurrentGuildId != null && Guilds.ContainsKey(App.CurrentGuildId) ? Guilds[App.CurrentGuildId] : null;
        public static GuildChannel CurrentChannel => CurrentGuild != null && CurrentGuild.channels.ContainsKey(App.CurrentChannelId) ? CurrentGuild.channels[App.CurrentChannelId] : null;
        public static Presence CurrentUserPresence => PresenceDict.ContainsKey(LocalState.CurrentUser.Id) ? PresenceDict[LocalState.CurrentUser.Id] : null;
    }
}
