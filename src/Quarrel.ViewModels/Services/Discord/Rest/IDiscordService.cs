// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Activities;
using DiscordAPI.API.Channel;
using DiscordAPI.API.Connections;
using DiscordAPI.API.Game;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Guild;
using DiscordAPI.API.Invite;
using DiscordAPI.API.Login;
using DiscordAPI.API.Misc;
using DiscordAPI.API.User;
using DiscordAPI.API.Voice;
using DiscordAPI.Models;
using JetBrains.Annotations;
using Quarrel.ViewModels.Services.Gateway;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Discord.Rest
{
    /// <summary>
    /// The default <see langword="interface"/> for the a service that executes REST calls to Discord.
    /// </summary>
    public interface IDiscordService
    {
        /// <summary>
        /// Gets or sets the current <see cref="User"/> instance, if present.
        /// </summary>
        [NotNull]
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets the <see cref="IActivitiesService"/> instance to retrieve Activities page data.
        /// </summary>
        [NotNull]
        IActivitiesService ActivitesService { get; }

        /// <summary>
        /// Gets the <see cref="IChannelService"/> instance to retrieve Channel data.
        /// </summary>
        [NotNull]
        IChannelService ChannelService { get; }

        /// <summary>
        /// Gets the <see cref="IConnectionsService"/> instance to retrieve Oauth data.
        /// </summary>
        [NotNull]
        IConnectionsService ConnectionsService { get; }

        /// <summary>
        /// Gets the <see cref="IGameService"/> instance to retrieve Game data.
        /// </summary>
        [NotNull]
        IGameService GameService { get; }

        /// <summary>
        /// Gets the <see cref="IGatewayService"/> instance to retrieve Gateway data.
        /// </summary>
        [NotNull]
        IGatewayConfigService GatewayConfigService { get; }

        /// <summary>
        /// Gets the <see cref="IGuildService"/> instance to retrieve Guild data.
        /// </summary>
        [NotNull]
        IGuildService GuildService { get; }

        /// <summary>
        /// Gets the <see cref="IInviteService"/> instance to retrieve guild Invites data.
        /// </summary>
        [NotNull]
        IInviteService InviteService { get; }

        /// <summary>
        /// Gets the <see cref="ILoginService"/> instance to retrieve login data.
        /// </summary>
        [NotNull]
        ILoginService LoginService { get; }

        /// <summary>
        /// Gets the <see cref="IMiscService"/> instance to retrieve misc data.
        /// </summary>
        [NotNull]
        IMiscService MiscService { get; }

        /// <summary>
        /// Gets the <see cref="IUserService"/> instance to retrieve user data.
        /// </summary>
        [NotNull]
        IUserService UserService { get; }

        /// <summary>
        /// Gets the <see cref="IVoiceService"/> instance to retrieve Voice Channel data.
        /// </summary>
        [NotNull]
        IVoiceService VoiceService { get; }

        /// <summary>
        /// Logs into Discord.
        /// </summary>
        /// <param name="token">The access token.</param>
        /// <param name="storeToken">Whether or not to store the token for future sessions.</param>
        /// <returns>A value indicating whether or not login was successful.</returns>
        Task<bool> Login([NotNull] string token, bool storeToken = false);

        /// <summary>
        /// Logs into Discord.
        /// </summary>
        /// <param name="email">The email account of the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A value indicating whether or not login was successful.</returns>
        Task<bool> Login([NotNull] string email, [NotNull] string password);

        /// <summary>
        /// Logs out from Discord by dropping the access token.
        /// </summary>
        void Logout();
    }
}
