// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API;
using Discord.API.Models.Users;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Services.Storage.Accounts.Models;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService : IDiscordService
    {
        private DiscordClient _discordClient;
        private IMessenger _messenger;

        public DiscordService(IMessenger messenger)
        {
            _messenger = messenger;
            _discordClient = new DiscordClient();
            _discordClient.LoggedIn += OnLoggedIn;
        }

        /// <inheritdoc/>
        public async Task LoginAsync(string token)
        {
            await _discordClient.LoginAsync(token);

            _messenger.Send(new ConnectingMessage());
        }

        private void OnLoggedIn(object sender, SelfUser e)
        {
            string? token = _discordClient.Token;
            Guard.IsNotNull(token, nameof(token));
            AccountInfo info = new AccountInfo(e.Id, e.Username, e.Discriminator, token);

            _messenger.Send(new UserLoggedInMessage(info));
        }
    }
}
