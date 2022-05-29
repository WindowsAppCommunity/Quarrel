// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client;
using Quarrel.Client.Models.Users;
using Quarrel.Messages;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
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
        private readonly IClipboardService _clipboardService;
        private readonly ILocalizationService _localizationService;
        private readonly IDispatcherService _dispatcherService;
        private readonly IMessenger _messenger;
        private LoginType? _loginSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordService"/> class.
        /// </summary>
        public DiscordService(
            IAnalyticsService analyticsService,
            IClipboardService clipboardService,
            ILocalizationService localizationService,
            IDispatcherService dispatcherService,
            IMessenger messenger)
        {
            _analyticsService = analyticsService;
            _clipboardService = clipboardService;
            _localizationService = localizationService;
            _dispatcherService = dispatcherService;
            _messenger = messenger;

            _quarrelClient = new QuarrelClient();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _quarrelClient.LoggedIn += OnLoggedIn;
            _quarrelClient.LoggedOut += OnLoggedOut;
            _quarrelClient.Reconnecting += OnReconnecting;
            _quarrelClient.Resuming += OnResuming;
            _quarrelClient.HttpExceptionHandled += OnHttpExceptionHandled;
            _quarrelClient.GatewayExceptionHandled += OnGatewayExceptionHandled;

            _quarrelClient.UnknownGatewayOperationEncountered += OnUnknownGatewayOperationEncountered;
            _quarrelClient.UnknownGatewayEventEncountered += OnUnknownGatewayEventEncountered;
            _quarrelClient.KnownGatewayEventEncountered += OnKnownGatewayEventEncountered;
            _quarrelClient.UnhandledGatewayOperationEncountered += OnUnhandledGatewayOperationEncountered;
            _quarrelClient.UnhandledGatewayEventEncountered += OnUnhandledGatewayEventEncountered;

            RegisterChannelEvents();
        }

        /// <inheritdoc/>
        public async Task<bool> LoginAsync(string token, LoginType source = LoginType.Unspecified)
        {
            _messenger.Send(new ConnectingMessage());
            _analyticsService.Log(LoggedEvent.LoginAttempted);

            try
            {
                _loginSource = source;
                await _quarrelClient.LoginAsync(token);

                return true;
            }
            catch (Exception e)
            {
                // TODO: Report error with messenger.
                _analyticsService.Log(LoggedEvent.LoginFailed,
                    (nameof(source), $"{source}"),
                    ("Exception Type", e.GetType().FullName),
                    ("Exception Message", e.Message));

                return false;
            }
        }

        private void OnLoggedIn(object sender, SelfUser e)
        {
            _analyticsService.Log(LoggedEvent.SuccessfulLogin, (nameof(_loginSource), $"{_loginSource}"));

            string? token = _quarrelClient.Token;

            Guard.IsNotNull(token, nameof(token));
            var info = new AccountInfo(e.Id, e.Username, e.Discriminator, token);

            _messenger.Send(new UserLoggedInMessage(info));
        }

        private void OnLoggedOut()
        {
            _messenger.Send(new UserLoggedOutMessage());
        }

        private void OnReconnecting()
        {
            _messenger.Send(new ConnectingMessage());
        }

        private void OnResuming()
        {
            _messenger.Send(new ConnectingMessage());
        }

        private void OnHttpExceptionHandled(object sender, Exception e)
            => LogException(LoggedEvent.HttpExceptionHandled, e);

        private void OnGatewayExceptionHandled(object sender, Exception e)
            => LogException(LoggedEvent.GatewayExceptionHandled, e);

        private void OnUnknownGatewayOperationEncountered(object sender, int e)
        {
            _analyticsService.Log(LoggedEvent.UnknownGatewayOperationEncountered,
                ("Operation", $"{e}"));
        }

        private void OnUnknownGatewayEventEncountered(object sender, string e)
        {
            _analyticsService.Log(LoggedEvent.UnknownGatewayEventEncountered,
                ("Event", e));
        }

        private void OnKnownGatewayEventEncountered(object sender, string e)
        {
            _analyticsService.Log(LoggedEvent.KnownGatewayEventEncountered,
                ("Event", e));
        }

        private void OnUnhandledGatewayOperationEncountered(object sender, int e)
        {
            _analyticsService.Log(LoggedEvent.UnhandledGatewayOperationEncountered,
                ("Operation", $"{e}"));
        }

        private void OnUnhandledGatewayEventEncountered(object sender, string e)
        {
            _analyticsService.Log(LoggedEvent.UnhandledGatewayEventEncountered,
                ("Event", e));
        }

        private void LogException(LoggedEvent type, Exception e)
        {
            _analyticsService.Log(type,
                    ("Exception Type", e.GetType().FullName),
                    ("Exception Message", e.Message));
        }
    }
}
