// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API;
using Discord.API.Models.Users;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Services.Storage.Accounts.Models;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    /// <summary>
    /// A service for handling discord client state and requests.
    /// </summary>
    public partial class DiscordService : IDiscordService
    {
        private readonly DiscordClient _discordClient;
        private readonly IMessenger _messenger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordService"/> class.
        /// </summary>
        public DiscordService(IMessenger messenger)
        {
            _messenger = messenger;
            _discordClient = new DiscordClient();
            _discordClient.LoggedIn += OnLoggedIn;
        }

        /// <inheritdoc/>
        public async Task<bool> LoginAsync(string token)
        {
            _messenger.Send(new ConnectingMessage());
            var success = await _discordClient.LoginAsync(token);
            // TODO: Report error.
            return success;
        }

        private void OnLoggedIn(object sender, SelfUser e)
        {
            string? token = _discordClient.Token;
            
            Guard.IsNotNull(token, nameof(token));
            var info = new AccountInfo(e.Id, e.Username, e.Discriminator, token);

            _messenger.Send(new UserLoggedInMessage(info));
        }
    }
}
