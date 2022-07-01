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
using System.Collections.Generic;
using System.Threading.Tasks;
using Webrtc;

namespace Quarrel.Services.Discord
{
    /// <summary>
    /// A service for handling discord client state and requests.
    /// </summary>
    public partial class DiscordService : IDiscordService
    {
        private readonly QuarrelClient _quarrelClient;
        private readonly ILoggingService _loggingService;
        private readonly IClipboardService _clipboardService;
        private readonly ILocalizationService _localizationService;
        private readonly IDispatcherService _dispatcherService;
        private readonly IMessenger _messenger;
        private WebrtcManager? _manager;
        public Dictionary<string, WebrtcManager> Streams { get; } = new Dictionary<string, WebrtcManager>();

        private LoginType? _loginSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordService"/> class.
        /// </summary>
        public DiscordService(
            ILoggingService loggingService,
            IClipboardService clipboardService,
            ILocalizationService localizationService,
            IDispatcherService dispatcherService,
            IMessenger messenger,
            QuarrelClient quarrelClient)
        {
            _loggingService = loggingService;
            _clipboardService = clipboardService;
            _localizationService = localizationService;
            _dispatcherService = dispatcherService;
            _messenger = messenger;

            _quarrelClient = quarrelClient;
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _quarrelClient.LoggedIn += OnLoggedIn;
            _quarrelClient.LoggedOut += OnLoggedOut;
            _quarrelClient.Reconnecting += OnReconnecting;
            _quarrelClient.Resuming += OnResuming;
            
            _quarrelClient.Voice.Ready = (string IP, ushort port, uint SSRC) =>
            {
                _manager = new WebrtcManager();
                _manager.IpAndPortObtained = _quarrelClient.Voice.SelectProtocol;
                _manager.Speaking = _quarrelClient.Voice.SendSpeaking;
                _manager.AudioInData = (IList<float> data) => { };
                _manager.AudioOutData = (IList<float> data) => { };
                _manager.Connect(IP, port.ToString(), SSRC);
            };

            _quarrelClient.Voice.SessionDescription = (string? audioCodec, string mediaSessionId, string mode, byte[] secretKey, string? videoCodec) =>
            {
                Guard.IsNotNull(_manager, nameof(_manager));
                _manager!.SetKey(secretKey);
            };
            
            _quarrelClient.Voice.Speaking = (string userId, uint SSRC, int isSpeaking) =>
            {
                Guard.IsNotNull(_manager, nameof(_manager));
                _manager.SetSpeaking(SSRC, isSpeaking);
            };

            _quarrelClient.Voice.Disconnected = () =>
            {
                _manager?.Destroy();
                _manager = null;
            };

            _quarrelClient.StreamCreated += (object sender, string streamKey) =>
            {
                WebrtcManager? manager = null;
                _quarrelClient.Voice.StreamConnections[streamKey].Ready = (string IP, ushort port, uint SSRC) =>
                {
                    manager = new WebrtcManager();
                    manager.IpAndPortObtained = _quarrelClient.Voice.StreamConnections[streamKey].SelectProtocol;
                    manager.Speaking = _quarrelClient.Voice.StreamConnections[streamKey].SendSpeaking;
                    manager.AudioInData = (IList<float> data) => { };
                    manager.AudioOutData = (IList<float> data) => { };
                    manager.Connect(IP, port.ToString(), SSRC);

                    Streams[streamKey] = manager;
                };

                _quarrelClient.Voice.StreamConnections[streamKey].SessionDescription = (string? audioCodec, string mediaSessionId, string mode, byte[] secretKey, string? videoCodec) =>
                {
                    Guard.IsNotNull(manager, nameof(manager));
                    manager!.SetKey(secretKey);
                };

                _quarrelClient.Voice.StreamConnections[streamKey].Speaking = (string userId, uint SSRC, int isSpeaking) =>
                {
                    Guard.IsNotNull(manager, nameof(manager));
                    manager.SetSpeaking(SSRC, isSpeaking);
                };

                _quarrelClient.Voice.StreamConnections[streamKey].Video = (ulong userId, uint SSRC) =>
                {
                    manager.SetVideoStream(userId, SSRC);
                };

                _quarrelClient.Voice.StreamConnections[streamKey].Disconnected = () =>
                {
                    manager?.Destroy();
                    manager = null;
                    Streams.Remove(streamKey);
                };
            };

            RegisterChannelEvents();
        }

        /// <inheritdoc/>
        public async Task<bool> LoginAsync(string token, LoginType source = LoginType.Unspecified)
        {
            _messenger.Send(new ConnectingMessage());
            _loggingService.Log(LoggedEvent.LoginAttempted);

            try
            {
                _loginSource = source;
                await _quarrelClient.LoginAsync(token);

                return true;
            }
            catch (Exception e)
            {
                // TODO: Report error with messenger.
                _loggingService.Log(LoggedEvent.LoginFailed,
                    (nameof(source), $"{source}"),
                    ("Exception Type", e.GetType().FullName),
                    ("Exception Message", e.Message));

                return false;
            }
        }

        private void OnLoggedIn(object sender, SelfUser e)
        {
            _loggingService.Log(LoggedEvent.SuccessfulLogin, (nameof(_loginSource), $"{_loginSource}"));

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
    }
}
