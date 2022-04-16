// Quarrel © 2022

using Discord.API.Models.Enums.Settings;
using Discord.API.Models.Json.Settings;
using Quarrel.Client.Models.Base;

namespace Quarrel.Client.Models.Settings
{
    /// <summary>
    /// User settings managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class Settings : DiscordItem
    {
        internal Settings(JsonUserSettings jsonUserSettings, QuarrelClient context) :
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

            switch (jsonUserSettings.Theme)
            {
                case "dark":
                    Theme = Theme.Dark;
                    break;
                case "light":
                    Theme = Theme.Light;
                    break;
            }

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
        /// Gets the Discord theme for the user.
        /// </summary>
        public Theme Theme { get; private set; }
    }
}
