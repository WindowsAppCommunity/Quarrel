// Quarrel © 2022

using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using System.Collections.Generic;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// An artifical guild item for selecting DMs.
    /// </summary>
    public class BindableHomeItem : SelectableItem, IBindableSelectableGuildItem, IBindableGuildListItem
    {
        private const string HomeResouece = "Guilds/Home";
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Initializes a new isntance of the <see cref="BindableHomeItem"/> class.
        /// </summary>
        public BindableHomeItem(IDiscordService discordService, IDispatcherService dispatcherService, ILocalizationService localizationService) :
            base(discordService, dispatcherService)
        {
            _localizationService = localizationService;
        }

        /// <inheritdoc/>
        public string? Name => _localizationService[HomeResouece];

        /// <inheritdoc/>
        public ulong? SelectedChannelId { get; set; }

        /// <inheritdoc/>
        public IEnumerable<BindableChannelGroup>? GetGroupedChannels(out IBindableSelectableChannel? selected)
        {
            var channels = _discordService.GetPrivateChannels(this, out selected);
            var group = new BindableChannelGroup(_discordService, _dispatcherService, null);
            foreach (var channel in channels)
            {
                if (channel is not null)
                {
                    group.AddChild(channel);
                }
            }

            return new BindableChannelGroup[] { group };
        }
    }
}
