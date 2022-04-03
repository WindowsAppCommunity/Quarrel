// Adam Dernis © 2022

using Discord.API.Models.Enums.Settings;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Base;

namespace Discord.API.Models.Settings
{
    public class Settings : DiscordItem
    {
        internal Settings(JsonUserSettings jsonUserSettings, DiscordClient context) :
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
                Folders[i] = new GuildFolder(jsonUserSettings.GuildFolders[i]);
            }
        }

        public ulong[] GuildOrder { get; private set; }

        public GuildFolder[] Folders { get; private set; }

        public bool IsDeveloperMode { get; private set; }

        public bool RenderReactions { get; private set; }

        public bool RenderEmbeds { get; private set; }

        public bool InlineEmbedMedia { get; private set; }

        public bool InlineAttachementMedia { get; private set; }

        public string Locale { get; private set; }

        public bool ShowCurrentGame { get; private set; }

        public Theme Theme { get; private set; }
    }
}
