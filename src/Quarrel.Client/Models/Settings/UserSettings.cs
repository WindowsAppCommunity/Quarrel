// Quarrel © 2022

using Discord.API.Models.Enums.Settings;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Settings;
using Quarrel.Client.Models.Base;

namespace Quarrel.Client.Models.Settings
{
    /// <summary>
    /// User settings managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class UserSettings : DiscordItem
    {
        internal UserSettings(JsonUserSettings jsonUserSettings, QuarrelClient context) :
            base(context)
        {
            GuildOrder = jsonUserSettings.GuildOrder;
            IsDeveloperMode = jsonUserSettings.DeveloperMode;
            RenderReactions = jsonUserSettings.RenderReactions;
            RenderEmbeds = jsonUserSettings.RenderEmbeds;
            InlineEmbedMedia = jsonUserSettings.InlineEmbedMedia;
            InlineAttachementMedia = jsonUserSettings.InlineAttachementMedia;
            Locale = jsonUserSettings.Locale;
            ShowCurrentGame = jsonUserSettings.ShowCurrentGame;
            ContentFilterLevel = jsonUserSettings.ExplicitContentFilter;
            RestrictedGuilds = jsonUserSettings.RestrictedGuilds;

            Status = jsonUserSettings.Status switch
            {
                "online" => UserStatus.Online,
                "idle" => UserStatus.Idle,
                "afk" => UserStatus.AFK,
                "dnd" => UserStatus.DoNotDisturb,
                "invisible" => UserStatus.Invisible,
                "offline" or _ => UserStatus.Offline,
            };

            Theme = jsonUserSettings.Theme switch
            {
                "light" => Theme.Light,
                "dark" or _ => Theme.Dark,
            };

            Folders = new GuildFolder[jsonUserSettings.GuildFolders.Length];
            for (int i = 0; i < jsonUserSettings.GuildFolders.Length; i++)
            {
                Folders[i] = new GuildFolder(jsonUserSettings.GuildFolders[i], context);
            }
        }

        /// <summary>
        /// Gets the order of guilds for the user.
        /// </summary>
        public ulong[] GuildOrder { get; private set; }

        /// <summary>
        /// Gets the folders in a guild.
        /// </summary>
        public GuildFolder[] Folders { get; private set; }

        /// <summary>
        /// Gets the guilds that don't allow users to DM the user.
        /// </summary>
        public ulong[] RestrictedGuilds { get; private set; }

        /// <summary>
        /// Gets if the user is in developer mode.
        /// </summary>
        public bool IsDeveloperMode { get; private set; }

        /// <summary>
        /// Gets if reactions are shown.
        /// </summary>
        public bool RenderReactions { get; private set; }

        /// <summary>
        /// Gets if embeds are shown.
        /// </summary>
        public bool RenderEmbeds { get; private set; }

        /// <summary>
        /// Gets if embeds are displayed inline.
        /// </summary>
        public bool InlineEmbedMedia { get; private set; }

        /// <summary>
        /// Gets if attachments are displayed inline.
        /// </summary>
        public bool InlineAttachementMedia { get; private set; }

        /// <summary>
        /// Gets the user's locale.
        /// </summary>
        public string Locale { get; private set; }

        /// <summary>
        /// Gets if the user's presence includes the current game.
        /// </summary>
        public bool ShowCurrentGame { get; private set; }

        /// <summary>
        /// Gets if the user's presence includes the current game.
        /// </summary>
        public ExplicitContentFilterLevel ContentFilterLevel { get; private set; }

        /// <summary>
        /// Gets the Discord theme for the user.
        /// </summary>
        public Theme Theme { get; private set; }

        /// <summary>
        /// Gets the online status for the user.
        /// </summary>
        public UserStatus Status { get; private set; }
    }
}
