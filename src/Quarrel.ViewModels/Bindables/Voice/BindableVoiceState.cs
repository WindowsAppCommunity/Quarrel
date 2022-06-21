// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Client.Models.Voice;
using Quarrel.Messages.Discord.Voice;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Voice
{
    /// <summary>
    /// A wrapper of a <see cref="VoiceState"/> that can be bound to the UI.
    /// </summary>
    public class BindableVoiceState : BindableItem
    {
        private VoiceState _state;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableVoiceState"/> class.
        /// </summary>
        internal BindableVoiceState(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            VoiceState state) :
            base(messenger, discordService, dispatcherService)
        {
            _state = state;

            _messenger.Register<VoiceStateUpdatedMessage>(this, (_, m) =>
            {
                if (m.VoiceState == State)
                {
                    OnPropertyChanged(nameof(State));
                }
            });
        }

        /// <summary>
        /// Gets the wrapped <see cref="VoiceState"/>.
        /// </summary>
        public VoiceState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
    }
}
