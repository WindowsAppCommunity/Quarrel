using DiscordAPI.Gateway.DownstreamEvents;
using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Guilds;
using DiscordAPI.Models.Messages;
using System;
using System.Threading.Tasks;

namespace DiscordAPI.Gateway
{
    public interface IGatewayService
    {
        event EventHandler<GatewayEventArgs<Ready>> Ready;
        event EventHandler<GatewayEventArgs<InvalidSession>> InvalidSession;
        event EventHandler<GatewayEventArgs<Resumed>> Resumed;

        event EventHandler<GatewayEventArgs<Guild>> GuildCreated;
        event EventHandler<GatewayEventArgs<Guild>> GuildUpdated;
        event EventHandler<GatewayEventArgs<GuildDelete>> GuildDeleted;

        event EventHandler<GatewayEventArgs<Channel>> ChannelCreated;
        event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelUpdated;
        event EventHandler<GatewayEventArgs<Channel>> ChannelDeleted;

        event EventHandler<GatewayEventArgs<Message>> MessageCreated;
        event EventHandler<GatewayEventArgs<Message>> MessageUpdated;
        event EventHandler<GatewayEventArgs<MessageDelete>> MessageDeleted;

        event EventHandler<GatewayEventArgs<GuildMemberAdd>> GuildMemberAdded;
        event EventHandler<GatewayEventArgs<GuildMemberRemove>> GuildMemberRemoved;
        event EventHandler<GatewayEventArgs<GuildMemberUpdate>> GuildMemberUpdated;
        event EventHandler<GatewayEventArgs<GuildMembersChunk>> GuildMembersChunk;

        event EventHandler<GatewayEventArgs<Presence>> PresenceUpdated;
        event EventHandler<GatewayEventArgs<TypingStart>> TypingStarted;

        Task<bool> ConnectAsync();
        Task<bool> ResumeAsync();
    }
}
