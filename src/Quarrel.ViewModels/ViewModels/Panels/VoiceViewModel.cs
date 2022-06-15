// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client.Models.Voice;
using Quarrel.Messages.Discord.Voice;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for the active voice channel in the app.
    /// </summary>
    public partial class VoiceViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(IsConnected))]
        private VoiceState? _voiceState;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceViewModel"/> class.
        /// </summary>
        public VoiceViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            _messenger.Register<VoiceStateUpdatedMessage>(this, (_, m) =>
            {
                if (m.VoiceState.User.Id != _discordService.GetMe()!.User.Id) return;

                _dispatcherService.RunOnUIThread(() => VoiceState = m.VoiceState);
            });

            HangupCommand = new RelayCommand(Hangup);
        }

        /// <summary>
        /// A command that hangs up the active call.
        /// </summary>
        public RelayCommand HangupCommand { get; }

        /// <summary>
        /// Gets whether or not the user is connected to a call.
        /// </summary>
        public bool IsConnected => _voiceState?.Channel != null;

        private void Hangup() => _discordService.LeaveCall();
    }
}
