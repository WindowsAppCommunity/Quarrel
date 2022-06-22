// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Bindables.Voice;
using Quarrel.Client.Models.Channels.Interfaces;
using System.Collections.ObjectModel;

namespace Quarrel.Bindables.Channels.Interfaces
{
    /// <summary>
    /// An interface for wrapping a <see cref="IAudioChannel"/> in a bindable context.
    /// </summary>
    public interface IBindableAudioChannel : IBindableChannel
    {
        /// <summary>
        /// Gets a value indicating whether or not the voice channel is currently connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the wrapped <see cref="IAudioChannel"/>.
        /// </summary>
        IAudioChannel AudioChannel { get; }

        /// <summary>
        /// Gets a command that joins the call in the channel.
        /// </summary>
        RelayCommand JoinCallCommand { get; }
        
        /// <summary>
        /// Gets the voice channel members.
        /// </summary>
        ObservableCollection<BindableVoiceState> VoiceMembers { get; }

        /// <summary>
        /// Connects to the audio channel.
        /// </summary>
        void JoinCall();
    }
}
