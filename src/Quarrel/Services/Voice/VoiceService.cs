// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.Messages.Services.Settings;
using Quarrel.Services.Settings.Enums;
using Quarrel.Services.Voice.Audio;

namespace Quarrel.Services.Voice
{
    public sealed class VoiceService : IVoiceService
    {
        public IAudioService InAudioService { get; } = new AudioService();

        public IAudioService OutAudioService { get; } = new AudioService();
    }
}