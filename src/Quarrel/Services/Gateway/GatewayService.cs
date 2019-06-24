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
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Gateway
{
    public class GatewayService : IGatewayService
    {
        public DiscordAPI.Gateway.Gateway Gateway { get; private set; }

        public async void InitializeGateway([NotNull] string accessToken)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            IGatewayConfigService gatewayService = restFactory.GetGatewayConfigService();

            GatewayConfig gatewayConfig = await gatewayService.GetGatewayConfig();
            IAuthenticator authenticator = new DiscordAuthenticator(accessToken);

            Gateway = new DiscordAPI.Gateway.Gateway(gatewayConfig, authenticator);

            Gateway.Ready += Gateway_Ready;
            Gateway.GuildMemberChunk += Gateway_GuildMemberChunk;
            Gateway.GuildSynced += Gateway_GuildSynced;

            Gateway.PresenceUpdated += Gateway_PresenceUpdated;
            Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
            Gateway.UserSettingsUpdated += Gateway_UserSettingsUpdated;
            
            await Gateway.ConnectAsync();

            Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
            {
                // TODO: Channel typing check
                //var channelList = ServicesManager.Cache.Runtime.TryGetValue<List<Channel>>(Quarrel.Helpers.Constants.Cache.Keys.ChannelList, m.GuildId);
                //var idList = channelList.ConvertAll(x => x.Id);

                List<string> idList = new List<string>();
                idList.Add(m.GuildId);

                await Gateway.SubscribeToGuild(idList.ToArray());
            });
        }

        private void Gateway_UserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> e)
        {
            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Presence, new Presence() { Status = e.EventData.Status }, 
                ServicesManager.Cache.Runtime.TryGetValue<BindableGuildMember>(Quarrel.Helpers.Constants.Cache.Keys.CurrentUser).Model.User.Id);
            Messenger.Default.Send(new GatewayUserSettingsUpdatedMessage());
        }

        #region Events

        private void Gateway_Ready(object sender, GatewayEventArgs<Ready> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayReadyMessage());
        }

        private void Gateway_GuildMemberChunk(object sender, GatewayEventArgs<GuildMemberChunk> e)
        {
            e.EventData.Cache();
        }

        private void Gateway_GuildSynced(object sender, GatewayEventArgs<GuildSync> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayGuildSyncMessage(e.EventData.GuildId));
        }

        private void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Presence, e.EventData, e.EventData.User.Id);
            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(e.EventData.User.Id));
        }

        private void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Note, e.EventData.Note, e.EventData.UserId);
            Messenger.Default.Send(new GatewayNoteUpdatedMessage(e.EventData.UserId));
        }

        #endregion
    }
}
