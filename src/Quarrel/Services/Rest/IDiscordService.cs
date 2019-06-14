using JetBrains.Annotations;
using DiscordAPI.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.ServiceModel.Channels;
using Quarrel.Gateway;

namespace Quarrel.Services.Rest
{
    /// <summary>
    /// The default <see langword="interface"/> for the a service that executes REST calls to Discord
    /// </summary>
    internal interface IDiscordService
    {
        /// <summary>
        /// Gets the current <see cref="User"/> instance, if present
        /// </summary>
        [NotNull]
        User CurrentUser { get; }

        /// <summary>
        /// Gets the <see cref="IActivitesService"/> instance to retrieve Activities page data
        /// </summary>
        [NotNull]
        IActivitesService ActivitesService { get; }

        /// <summary>
        /// Gets the <see cref="IChannelService"/> instance to retrieve Channel data
        /// </summary>
        [NotNull]
        IChannelService ChannelService { get; }

        /// <summary>
        /// Gets the <see cref="IConnectionsService"/> instance to retrieve Oauth data
        /// </summary>
        [NotNull]
        IConnectionsService ConnectionsService { get; }

        /// <summary>
        /// Gets the <see cref="IGameService"/> instance to retrieve Game data
        /// </summary>
        [NotNull]
        IGameService GameService { get; }

        /// <summary>
        /// Gets the <see cref="IGatewayService"/> instance to retrieve Gateway data
        /// </summary>
        [NotNull]
        IGatewayService GatewayService { get; }

        /// <summary>
        /// Gets the <see cref="IGuildService"/> instance to retrieve Guild data
        /// </summary>
        [NotNull]
        IGuildService GuildService { get; }

        /// <summary>
        /// Gets the <see cref="IInviteService"/> instance to retrieve guild Invites data
        /// </summary>
        [NotNull]
        IInviteService InviteService { get; }

        /// <summary>
        /// Gets the <see cref="ILoginService"/> instance to retrieve login data
        /// </summary>
        [NotNull]
        ILoginService LoginService { get; }

        /// <summary>
        /// Gets the <see cref="IMiscService"/> instance to retrieve misc data
        /// </summary>
        [NotNull]
        IMiscService MiscService { get; }

        /// <summary>
        /// Gets the <see cref="IUserService"/> instance to retrieve user data
        /// </summary>
        [NotNull]
        IUserService UserService { get; }

        /// <summary>
        /// Gets the <see cref="IVoiceService"/> instance to retrieve Voice Channel data
        /// </summary>
        [NotNull]
        IVoiceService VoiceService { get; }
    }
}
