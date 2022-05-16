// Quarrel © 2022

using Discord.API.Models.Enums.Users;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Messages;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Messages;
using Quarrel.Services.Analytics.Enums;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    /// <summary>
    /// An interface for a service that handles interactions with the discord client.
    /// </summary>
    public interface IDiscordService
    {
        /// <summary>
        /// Gets the current user for the <see cref="DiscordService"/>.
        /// </summary>
        /// <returns>The current user as a <see cref="BindableSelfUser"/>.</returns>
        BindableSelfUser? GetMe();

        /// <summary>
        /// Gets a user by id.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <returns>The user of an id.</returns>
        BindableUser? GetUser(ulong userId);

        /// <summary>
        /// Logs into the discord service by token.
        /// </summary>
        /// <param name="token">The token to use for login.</param>
        /// <param name="source">The login source.</param>
        Task<bool> LoginAsync(string token, LoginType source = LoginType.Unspecified);
        
        /// <summary>
        /// Gets the current user's guilds.
        /// </summary>
        /// <returns>The array of <see cref="BindableGuild"/>s that the current user is in.</returns>
        BindableGuild[] GetMyGuilds();
        
        /// <summary>
        /// Gets the current user's guild folders with children.
        /// </summary>
        /// <remarks>
        /// Contains null folders, whose children should be treated as though they're in the roots.
        /// </remarks>
        /// <returns>The array of <see cref="BindableGuildFolder"/>s that the current user has.</returns>
        BindableGuildFolder[] GetMyGuildFolders();

        /// <summary>
        /// Gets the messages in a channel.
        /// </summary>
        /// <param name="channel">The channel to get the messages for.</param>
        /// <param name="beforeId">The if of the last message to load messages before, or null.</param>
        /// <returns>An array of <see cref="BindableMessage"/>s from the channel.</returns>
        Task<Message[]> GetChannelMessagesAsync(IBindableMessageChannel channel, ulong? beforeId = null);

        /// <summary>
        /// Gets the channels in a guild.
        /// </summary>
        /// <param name="guild">The guild to get the channels for.</param>
        /// <param name="selectedChannel">The selected channel as an <see cref="IBindableSelectableChannel"/>.</param>
        /// <returns>An array of <see cref="BindableGuildChannel"/>s from the guild.</returns>
        BindableGuildChannel?[] GetGuildChannels(BindableGuild guild, out IBindableSelectableChannel? selectedChannel);

        /// <summary>
        /// Gets the user's direct message channels.
        /// </summary>
        /// <param name="home">The <see cref="BindableHomeItem"/>.</param>
        /// <param name="selectedChannel">The selected channel as an <see cref="IBindableSelectableChannel"/>.</param>
        /// <returns>An array of <see cref="BindablePrivateChannel"/>s.</returns>
        BindablePrivateChannel?[] GetPrivateChannels(BindableHomeItem home, out IBindableSelectableChannel? selectedChannel);

        BindableGuildMember? GetGuildMember(ulong userId, ulong guildId);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="channelId">The id of the channel to send the message in.</param>
        /// <param name="content">The content of the message.</param>
        Task SendMessage(ulong channelId, string content);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="channelId">The id of the channel the message is in.</param>
        /// <param name="messageId">The id of the message to delete.</param>
        Task DeleteMessage(ulong channelId, ulong messageId);

        /// <summary>
        /// Updates the user's online status.
        /// </summary>
        /// <param name="status">The new online status to set.</param>
        Task SetStatus(UserStatus status);
    }
}
