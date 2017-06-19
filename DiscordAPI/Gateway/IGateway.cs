using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway
{
    public interface IGateway
    {
        event EventHandler<GatewayEventArgs<Ready>> Ready;
        event EventHandler<GatewayEventArgs<Resumed>> Resumed;

        event EventHandler<GatewayEventArgs<Guild>> GuildCreated;
        event EventHandler<GatewayEventArgs<Guild>> GuildUpdated;
        event EventHandler<GatewayEventArgs<GuildDelete>> GuildDeleted;

        event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelCreated;
        event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelUpdated;
        event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelDeleted;

        event EventHandler<GatewayEventArgs<DirectMessageChannel>> DirectMessageChannelCreated;
        event EventHandler<GatewayEventArgs<DirectMessageChannel>> DirectMessageChannelDeleted;

        event EventHandler<GatewayEventArgs<Message>> MessageCreated;
        event EventHandler<GatewayEventArgs<Message>> MessageUpdated;
        event EventHandler<GatewayEventArgs<MessageDelete>> MessageDeleted;

        event EventHandler<GatewayEventArgs<Presence>> PresenceUpdated;
        event EventHandler<GatewayEventArgs<TypingStart>> TypingStarted;

        Task ConnectAsync();
        Task ResumeAsync();
    }
}
