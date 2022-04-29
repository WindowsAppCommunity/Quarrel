// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Messages;
using Quarrel.Messages.Discord;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        public void RegisterChannelEvents()
        {
            _quarrelClient.MessageUpdated += OnMessageUpdated;
            _quarrelClient.ChannelUpdated += OnChannelUpdated;
        }

        private void OnMessageUpdated(object sender, Message e)
        {
            _messenger.Send(new MessageUpdatedMessage(e));
        }

        private void OnChannelUpdated(object sender, Channel e)
        {
            _messenger.Send(new ChannelUpdatedMessage(e));
        }
    }
}
