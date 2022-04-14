// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Guilds;
using Discord.API.Models.Users;
using Discord.API.Rest;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Discord.API
{
    /// <summary>
    /// A Discord client instance containing a Token, Gateway, and Cache.
    /// </summary>
    public partial class DiscordClient
    {
        private IChannelService? _channelService;
        private IGatewayService? _gatewayService;
        private Gateway? _gateway;
        private string? _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordClient"/> class.
        /// </summary>
        public DiscordClient()
        {
            _guildMap = new ConcurrentDictionary<ulong, Guild>();
            _channelMap = new ConcurrentDictionary<ulong, Channel>();
            _userMap = new ConcurrentDictionary<ulong, User>();
            _guildsMemberMap = new ConcurrentDictionary<(ulong GuildId, ulong UserId), GuildMember>();
        }

        /// <summary>
        /// Gets the token used for authentication.
        /// </summary>
        public string? Token => _token;

        /// <summary>
        /// Initializes authenticated services and opens the gateway.
        /// </summary>
        /// <param name="token">The token used for authentication.</param>
        public async Task LoginAsync(string token)
        {
            _token = token;
            InitializeServices(token);
            await SetupGatewayAsync(token);
        }

        private void InitializeServices(string token)
        {
            var restFactory = new DiscordRestFactory
            {
                Token = token
            };
            _channelService = restFactory.GetChannelService();
            _gatewayService = restFactory.GetGatewayService();
        }

        private async Task SetupGatewayAsync(string token)
        {
            Guard.IsNotNull(_gatewayService, nameof(_gatewayService));

            var gatewayConfig = await _gatewayService.GetGatewayConfig();
            _gateway = new Gateway(gatewayConfig, token);
            await _gateway.ConnectAsync();
            RegisterEvents();
        }
    }
}
