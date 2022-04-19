// Quarrel © 2022

using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Messages;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Guilds;
using Quarrel.Services.Analytics.Enums;
using System.Collections.Generic;
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
        /// <returns>An array of <see cref="BindableMessage"/>s from the channel.</returns>
        Task<BindableMessage[]> GetChannelMessagesAsync(IBindableMessageChannel channel);
        
        /// <summary>
        /// Gets the channels in a guild.
        /// </summary>
        /// <param name="guild">The guild to get the channels for.</param>
        /// <returns>An array of <see cref="IBindableChannel"/>s from the guild.</returns>
        /// <param name="selectedChannel">The selected channel as an <see cref="IBindableSelectableChannel"/>.</param>
        BindableGuildChannel?[] GetGuildChannels(BindableGuild guild, out IBindableSelectableChannel? selectedChannel);

        /// <summary>
        /// Gets the channels in a guild as channel groups by category.
        /// </summary>
        /// <param name="guild">The <see cref="BindableGuild"/> to get the channels from.</param>
        /// <param name="selectedChannel">The selected channel as an <see cref="IBindableSelectableChannel"/>.</param>
        /// <returns>The <see cref="BindableChannel"/>s for the guild in category groups.</returns>
        IEnumerable<BindableChannelGroup>? GetGuildChannelsGrouped(BindableGuild guild, out IBindableSelectableChannel? selectedChannel);
    }
}
