// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Voice;
using Quarrel.Messages.Discord.Channels;
using Quarrel.Messages.Discord.Messages;
using Quarrel.Messages.Discord.Reactions;
using Quarrel.Messages.Discord.Stream;
using Quarrel.Messages.Discord.Voice;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        private void RegisterChannelEvents()
        {
            _quarrelClient.MessageCreated += OnMessageCreate;
            _quarrelClient.MessageUpdated += OnMessageUpdated;
            _quarrelClient.MessageDeleted += OnMessageDeleted;
            _quarrelClient.MessageAck += OnMessageAck;

            _quarrelClient.AllReactionsRemoved += OnAllReactionsRemoved;

            _quarrelClient.ChannelUpdated += OnChannelUpdated;

            _quarrelClient.VoiceStateAdded += OnVoiceStateAdded;
            _quarrelClient.VoiceStateUpdated += OnVoiceStateUpdated;
            _quarrelClient.VoiceStateRemoved += OnVoiceStateRemoved;

            _quarrelClient.StreamCreated += OnStreamCreated;
        }

        private void OnMessageCreate(object sender, Message e) 
            => _messenger.Send(new MessageCreatedMessage(e));

        private void OnMessageUpdated(object sender, Message e) 
            => _messenger.Send(new MessageUpdatedMessage(e));

        private void OnMessageDeleted(object sender, MessageDeleted e) 
            => _messenger.Send(new MessageDeletedMessage(e.ChannelId, e.MessageId));

        private void OnMessageAck(object sender, MessageAck e)
            => _messenger.Send(new MessageMarkedReadMessage(e.ChannelId, e.MessageId));

        private void OnAllReactionsRemoved(object sender, AllReactionsRemoved e)
            => _messenger.Send(new AllReactionsRemovedMessage(e.ChannelId, e.MessageId));

        private void OnChannelUpdated(object sender, Channel e) 
            => _messenger.Send(new ChannelUpdatedMessage(e));

        private void OnVoiceStateAdded(object sender, VoiceState e)
            => _messenger.Send(new VoiceStateAddedMessage(e));

        private void OnVoiceStateUpdated(object sender, VoiceState e)
        {
            _messenger.Send(new VoiceStateUpdatedMessage(e));

            if (e.User.Id == MyId)
            {
                _messenger.Send(new MyVoiceStateUpdatedMessage(e));
            }
        }

        private void OnVoiceStateRemoved(object sender, VoiceState e)
            => _messenger.Send(new VoiceStateRemovedMessage(e));
        
        private void OnStreamCreated(object sender, string streamKey) 
            => _messenger.Send(new StreamCreatedMessage(streamKey));
    }
}
 