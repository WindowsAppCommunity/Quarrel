// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Messages.Discord;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        public void RegisterChannelEvents()
        {
            _quarrelClient.ChannelUpdated += OnChannelUpdated;
        }

        private void OnChannelUpdated(object sender, Channel e)
        {
            _messenger.Send(new ChannelUpdatedMessage(e));
        }
    }
}
