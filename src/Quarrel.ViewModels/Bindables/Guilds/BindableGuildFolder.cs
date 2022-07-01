// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Client;
using Quarrel.Client.Models.Settings;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using System.Collections.ObjectModel;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// A wrapper of a <see cref="GuildFolder"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableGuildFolder : BindableItem, IBindableGuildListItem
    {
        [ObservableProperty]
        private GuildFolder _folder;

        internal BindableGuildFolder(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            IClipboardService clipboardService,
            ILocalizationService localizationService,
            GuildFolder folder) :
            base(messenger, discordService, quarrelClient, dispatcherService)
        {
            _folder = folder;

            var guilds = _folder.GetGuilds();
            Children = new ObservableCollection<BindableGuild>();
            foreach (var guild in guilds)
            {
                Children.Add(new BindableGuild(messenger, discordService, quarrelClient, dispatcherService, clipboardService, localizationService, guild));
            }
        }

        /// <summary>
        /// A collection of the guilds contained in the guild folder.
        /// </summary>
        public ObservableCollection<BindableGuild> Children { get; }
        
        /// <inheritdoc/>
        public string? Name => Folder.Name;
    }
}
