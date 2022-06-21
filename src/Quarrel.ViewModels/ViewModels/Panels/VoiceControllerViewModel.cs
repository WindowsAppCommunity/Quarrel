// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Voice;
using Quarrel.Messages.Discord.Voice;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for the active voice channel in the app.
    /// </summary>
    public partial class VoiceControllerViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(IsConnected))]
        private BindableVoiceState? _voiceState;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceControllerViewModel"/> class.
        /// </summary>
        public VoiceControllerViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            _messenger.Register<MyVoiceStateUpdatedMessage>(this, (_, m) =>
            {
                var state = new BindableVoiceState(
                    _messenger,
                    _discordService,
                    _dispatcherService,
                    m.VoiceState);

                _dispatcherService.RunOnUIThread(() => VoiceState = state);
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
        public bool IsConnected => _voiceState?.State.Channel != null;

        private void Hangup() => _discordService.LeaveCall();
    }
}
