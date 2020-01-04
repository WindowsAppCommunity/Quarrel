// Special thanks to Sergio Pedri for the basis of this design

using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using DiscordAPI.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.DownstreamEvents;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using DiscordAPI.API;
using DiscordAPI.API.Gateway;
using DiscordAPI.Authentication;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Cache;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Refit;
using Quarrel.ViewModels.Messages;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Channels;

namespace Quarrel.Services.Gateway
{
    public class GatewayService : IGatewayService
    {
        private ICacheService CacheService;
        private ICurrentUsersService CurrentUsersService;
        private IGuildsService GuildsService;
        public DiscordAPI.Gateway.Gateway Gateway { get; private set; }
        public IServiceProvider ServiceProvider { get; }
        public GatewayService(
            IServiceProvider serviceProvider,
            ICacheService cacheService, ICurrentUsersService currentUsersService, IGuildsService guildsService)
        {
            ServiceProvider = serviceProvider;
            CacheService = cacheService;
            CurrentUsersService = currentUsersService;
            GuildsService = guildsService;
        }

        public async Task<bool> InitializeGateway([NotNull] string accessToken)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            IGatewayConfigService gatewayService = restFactory.GetGatewayConfigService();

            try
            {
                GatewayConfig gatewayConfig = await gatewayService.GetGatewayConfig();
            IAuthenticator authenticator = new DiscordAuthenticator(accessToken);

            Gateway = new DiscordAPI.Gateway.Gateway(ServiceProvider, gatewayConfig, authenticator);
            }
            catch (Exception e)
            {
                Messenger.Default.Send(new StartUpStatusMessage(Status.Failed));
                return false;
            }

            Gateway.InvalidSession += Gateway_InvalidSession;
            Gateway.GatewayClosed += Gateway_GatewayClosed;

            Gateway.Ready += Gateway_Ready;
            Gateway.GuildMemberChunk += Gateway_GuildMemberChunk;
            Gateway.GuildSynced += Gateway_GuildSynced;

            Gateway.MessageCreated += Gateway_MessageCreated;
            Gateway.MessageDeleted += Gateway_MessageDeleted;
            Gateway.MessageUpdated += Gateway_MessageUpdated;
            Gateway.MessageAck += Gateway_MessageAck;

            Gateway.MessageReactionAdded += Gateway_MessageReactionAdded;
            Gateway.MessageReactionRemoved += Gateway_MessageReactionRemoved;
            Gateway.MessageReactionRemovedAll += Gateway_MessageReactionRemovedAll;

            Gateway.GuildChannelCreated += Gateway_GuildChannelCreated;
            Gateway.ChannelDeleted += Gateway_ChannelDeleted;
            Gateway.GuildChannelUpdated += Gateway_GuildChannelUpdated;

            Gateway.DirectMessageChannelCreated += Gateway_DirectMessageChannelCreated;

            Gateway.TypingStarted += Gateway_TypingStarted;

            Gateway.PresenceUpdated += Gateway_PresenceUpdated;
            Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
            Gateway.UserGuildSettingsUpdated += Gateway_UserGuildSettingsUpdated;
            Gateway.UserSettingsUpdated += Gateway_UserSettingsUpdated;

            Gateway.VoiceServerUpdated += Gateway_VoiceServerUpdated;
            Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;

            Gateway.SessionReplaced += Gateway_SessionReplaced;

            if (await ConnectWithRetryAsync(3))
            {
                Messenger.Default.Send(new StartUpStatusMessage(Status.Connected));
                Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
                {
                    // TODO: Channel typing check
                    //var channelList = ServicesManager.Cache.Runtime.TryGetValue<List<Channel>>(Quarrel.Helpers.Constants.Cache.Keys.ChannelList, m.GuildId);
                    //var idList = channelList.ConvertAll(x => x.Id);

                    List<string> idList = new List<string>();

                    // Guild Sync
                    if (m.Guild.Model.Id != "DM")
                    {
                        idList.Add(m.Guild.Model.Id);
                    }

                    await Gateway.SubscribeToGuild(idList.ToArray());
                });
            }

            return true;
        }

        public async Task<bool> ConnectWithRetryAsync(int retries)
        {
            for (int i = 0; i < retries; i++)
            {
                if (await Gateway.ConnectAsync()) return true;
            }

            return false;
        }


        #region Events

        private void Gateway_Ready(object sender, GatewayEventArgs<Ready> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayReadyMessage(e.EventData));
        }

        private void Gateway_InvalidSession(object sender, GatewayEventArgs<InvalidSession> e)
        {
            Messenger.Default.Send(new GatewayInvalidSessionMessage(e.EventData));
        }

        #region Messages

        private void Gateway_MessageCreated(object sender, GatewayEventArgs<Message> e)
        {
            var currentUser = CurrentUsersService.CurrentUser.Model;
            var channel = GuildsService.GetChannel(e.EventData.ChannelId);
            if (channel != null)
            {
                channel.UpdateLMID(e.EventData.Id);

                if (channel.IsDirectChannel || channel.IsGroupChannel || e.EventData.Mentions.Any(x => x.Id == currentUser.Id) ||
                    e.EventData.MentionEveryone)
                {
                    channel.ReadState.MentionCount++;
                    int oldIndex = GuildsService.Guilds["DM"].Channels.IndexOf(channel);
                    GuildsService.Guilds["DM"].Channels.Move(oldIndex, 0);
                }

            }

            if (e.EventData.User == null)
                e.EventData.User = currentUser;
            Messenger.Default.Send(new GatewayMessageRecievedMessage(e.EventData));
        }

        private void Gateway_MessageDeleted(object sender, GatewayEventArgs<MessageDelete> e)
        {
            Messenger.Default.Send(new GatewayMessageDeletedMessage(e.EventData.ChannelId, e.EventData.MessageId));
        }

        private void Gateway_MessageUpdated(object sender, GatewayEventArgs<Message> e)
        {
            Messenger.Default.Send(new GatewayMessageUpdatedMessage(e.EventData));
        }

        private void Gateway_MessageAck(object sender, GatewayEventArgs<MessageAck> e)
        {
            GuildsService.GetChannel(e.EventData.ChannelId)?.UpdateLRMID(e.EventData.Id);
            Messenger.Default.Send(new GatewayMessageAckMessage(e.EventData.ChannelId, e.EventData.Id));
        }

        #region Reactions

        private void Gateway_MessageReactionAdded(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            Messenger.Default.Send(new GatewayReactionAddedMessage(e.EventData.MessageId, e.EventData.ChannelId, e.EventData.Emoji, false));
        }

        private void Gateway_MessageReactionRemoved(object sender, GatewayEventArgs<MessageReactionUpdate> e)
        {
            Messenger.Default.Send(new GatewayReactionRemovedMessage(e.EventData.MessageId, e.EventData.ChannelId, e.EventData.Emoji));
        }

        private void Gateway_MessageReactionRemovedAll(object sender, GatewayEventArgs<MessageReactionRemoveAll> e)
        {
            Messenger.Default.Send(new GatewayReactionClearedMessage(e.EventData.MessageId, e.EventData.ChannelId));
        }

        #endregion

        #endregion

        #region Channels
        private void Gateway_GuildChannelCreated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            Messenger.Default.Send(new GatewayGuildChannelCreatedMessage(e.EventData));
        }

        private void Gateway_ChannelDeleted(object sender, GatewayEventArgs<Channel> e)
        {
            Messenger.Default.Send(new GatewayChannelDeletedMessage(e.EventData));
        }

        private void Gateway_GuildChannelUpdated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            Messenger.Default.Send(new GatewayGuildChannelUpdatedMessage(e.EventData));
        }

        private void Gateway_DirectMessageChannelCreated(object sender, GatewayEventArgs<DirectMessageChannel> e)
        {
            Messenger.Default.Send(new GatewayDirectMessageChannelCreatedMessage(e.EventData));
        }

        #endregion

        private void Gateway_TypingStarted(object sender, GatewayEventArgs<TypingStart> e)
        {
            Messenger.Default.Send(new GatewayTypingStartedMessage(e.EventData));
        }

        private void Gateway_GuildMemberChunk(object sender, GatewayEventArgs<GuildMemberChunk> e)
        {

        }

        private void Gateway_GuildSynced(object sender, GatewayEventArgs<GuildSync> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayGuildSyncMessage(e.EventData.GuildId, e.EventData.Members.ToList()));
        }

        private void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(e.EventData.User.Id, e.EventData));
        }

        private void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            CacheService.Runtime.SetValue(Constants.Cache.Keys.Note, e.EventData.Note, e.EventData.UserId);
            Messenger.Default.Send(new GatewayNoteUpdatedMessage(e.EventData.UserId));
        }

        private void Gateway_UserGuildSettingsUpdated(object sender, GatewayEventArgs<GuildSetting> e)
        {
            CurrentUsersService.GuildSettings.AddOrUpdate(e.EventData.GuildId ?? "DM", e.EventData);

            foreach (var channel in e.EventData.ChannelOverrides)
            {
                CurrentUsersService.ChannelSettings.AddOrUpdate(channel.ChannelId, channel);
            }

            Messenger.Default.Send(new GatewayUserGuildSettingsUpdatedMessage(e.EventData));
        }

        private void Gateway_UserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> e)
        {
            Messenger.Default.Send(new GatewayUserSettingsUpdatedMessage(e.EventData));
        }

        #region Voice 

        private void Gateway_VoiceServerUpdated(object sender, GatewayEventArgs<VoiceServerUpdate> e)
        {
            Messenger.Default.Send(new GatewayVoiceServerUpdateMessage(e.EventData));
        }

        private void Gateway_VoiceStateUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {
            Messenger.Default.Send(new GatewayVoiceStateUpdateMessage(e.EventData));
        }

        private void Gateway_SessionReplaced(object sender, GatewayEventArgs<SessionReplace[]> e)
        {
            Messenger.Default.Send(new GatewaySessionReplacedMessage(e.EventData));
        }

        private void Gateway_GatewayClosed(object sender, DiscordAPI.Sockets.WebSocketClosedException e)
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().Logout();
        }

        #endregion

        #endregion
    }
}
