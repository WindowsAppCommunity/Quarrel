// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client;
using Quarrel.Client.Models.Users;
using Quarrel.Messages.Discord;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Analytics.Models;
using Quarrel.Services.Storage.Accounts.Models;
using System;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    /// <summary>
    /// A service for handling discord client state and requests.
    /// </summary>
    public partial class DiscordService : IDiscordService
    {
        private readonly QuarrelClient _quarrelClient;
        private readonly IAnalyticsService _analyticsService;
        private readonly IMessenger _messenger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordService"/> class.
        /// </summary>
        public DiscordService(IAnalyticsService analyticsService, IMessenger messenger)
        {
            _analyticsService = analyticsService;
            _messenger = messenger;
            _quarrelClient = new QuarrelClient();
            _quarrelClient.LoggedIn += OnLoggedIn;
            _quarrelClient.HttpExceptionHandled += OnHttpExceptionHandled;
            _quarrelClient.GatewayExceptionHandled += OnGatewayExceptionHandled;
            
            _quarrelClient.UnknownGatewayOperationEncountered += OnUnknownGatewayOperationEncountered;
            _quarrelClient.UnknownGatewayEventEncountered += OnUnknownGatewayEventEncountered;
            _quarrelClient.KnownGatewayEventEncountered += OnKnownGatewayEventEncountered;
            _quarrelClient.UnhandledGatewayOperationEncountered += OnUnhandledGatewayOperationEncountered;
            _quarrelClient.UnhandledGatewayEventEncountered += OnUnhandledGatewayEventEncountered;
        }

        /// <inheritdoc/>
        public async Task<bool> LoginAsync(string token, LoginType source = LoginType.Unspecified)
        {
            _messenger.Send(new ConnectingMessage());
            try
            {
                await _quarrelClient.LoginAsync(token);
                _analyticsService.Log(LoggedEvent.SuccessfulLogin, (nameof(source), source));
                return true;
            }
            catch (Exception e)
            {
                // TODO: Report error with messenger.
                _analyticsService.Log(LoggedEvent.LoginFailed, (nameof(source), source), ("LoginError", new LoginError(e)));
                return false;
            }
        }

        private void OnLoggedIn(object sender, SelfUser e)
        {
            string? token = _quarrelClient.Token;

            Guard.IsNotNull(token, nameof(token));
            var info = new AccountInfo(e.Id, e.Username, e.Discriminator, token);

            _messenger.Send(new UserLoggedInMessage(info));
        }

        private void OnHttpExceptionHandled(object sender, Exception e)
            => LogException(LoggedEvent.HttpExceptionHandled, e);

        private void OnGatewayExceptionHandled(object sender, Exception e)
            => LogException(LoggedEvent.GatewayExceptionHandled, e);

        private void OnUnknownGatewayOperationEncountered(object sender, int e)
        {
            _analyticsService.Log(LoggedEvent.UnknownGatewayOperationEncountered, ("Operation", e));
        }

        private void OnUnknownGatewayEventEncountered(object sender, string e)
        {
            _analyticsService.Log(LoggedEvent.UnknownGatewayEventEncountered, ("Event", e));
        }

        private void OnKnownGatewayEventEncountered(object sender, string e)
        {
            _analyticsService.Log(LoggedEvent.KnownGatewayEventEncountered, ("Event", e));
        }

        private void OnUnhandledGatewayOperationEncountered(object sender, int e)
        {
            _analyticsService.Log(LoggedEvent.UnhandledGatewayOperationEncountered, ("Operation", e));
        }

        private void OnUnhandledGatewayEventEncountered(object sender, string e)
        {
            _analyticsService.Log(LoggedEvent.UnhandledGatewayEventEncountered, ("Event", e));
        }

        private void LogException(LoggedEvent type, Exception e)
            => _analyticsService.Log(type, ("Exception", e));
    }
}
