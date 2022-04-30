// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Messages;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Messages;
using Quarrel.Messages.Discord.Channels;
using Quarrel.Messages.Discord.Messages;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        public void RegisterChannelEvents()
        {
            _quarrelClient.MessageCreated += OnMessageCreate;
            _quarrelClient.MessageUpdated += OnMessageUpdated;
            _quarrelClient.ChannelUpdated += OnChannelUpdated;
        }

        private void OnMessageCreate(object sender, Message e)
        {
            BindableMessage message = new BindableMessage(_messenger, this, _dispatcherService, e);
            _messenger.Send(new MessageCreatedMessage(message));
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
