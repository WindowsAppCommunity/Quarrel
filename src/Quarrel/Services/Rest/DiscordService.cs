// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Services.Rest
{
    public class DiscordService : IDiscordService
    {
        #region Exposed services

        /// <inheritdoc/>
        public IActivitesService ActivitesService { get; private set; }

        /// <inheritdoc/>
        public IChannelService ChannelService { get; private set; }

        /// <inheritdoc/>
        public IConnectionsService ConnectionsService { get; private set; }

        /// <inheritdoc/>
        public IGameService GameService { get; private set; }

        /// <inheritdoc/>
        public IGatewayConfigService GatewayService { get; private set; }

        /// <inheritdoc/>
        public IGuildService GuildService { get; private set; }

        /// <inheritdoc/>
        public IInviteService InviteService { get; private set; }

        /// <inheritdoc/>
        public ILoginService LoginService { get; private set; }

        /// <inheritdoc/>
        public IMiscService MiscService { get; private set; }

        /// <inheritdoc/>
        public IUserService UserService { get; private set; }

        /// <inheritdoc/>
        public IVoiceService VoiceService { get; private set; }

        /// <inheritdoc/>
        public User CurrentUser { get; private set; }

        // The access token for the current user
        [NotNull]
        private string _AccessToken;

        #endregion

        #region Login



        #endregion
    }
}
