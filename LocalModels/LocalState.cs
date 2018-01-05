using System.Collections.Generic;
using Windows.UI.Xaml;
using Discord_UWP.SharedModels;

namespace Discord_UWP.LocalModels
{
    public class LocalState
    {
        public static Dictionary<string, Guild> Guilds = new Dictionary<string, Guild>();
        public static Dictionary<string, DirectMessageChannel> DMs = new Dictionary<string, DirectMessageChannel>();
        public static User CurrentUser = new User();
        public static Dictionary<string, Friend> Friends = new Dictionary<string, Friend>();
        public static Dictionary<string, string> Notes = new Dictionary<string, string>();

        public static Dictionary<TypingStart, DispatcherTimer> Typers = new Dictionary<TypingStart, DispatcherTimer>();
        public static List<DispatcherTimer> TyperTimers = new List<DispatcherTimer>();
        public static Dictionary<string, Presence> PresenceDict = new Dictionary<string, Presence>();
        public static Dictionary<string, VoiceState> VoiceDict = new Dictionary<string, VoiceState>();
        public static Dictionary<string, ReadState> RPC = new Dictionary<string, ReadState>();
        public static Dictionary<string, GuildSetting> GuildSettings = new Dictionary<string, GuildSetting>();
        public static VoiceState VoiceState = new VoiceState();
        public static List<DiscordAPI.API.Game.GameList> SupportedGames;
    }
}
