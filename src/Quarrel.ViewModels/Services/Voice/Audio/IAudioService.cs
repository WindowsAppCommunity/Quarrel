using System;

namespace Quarrel.ViewModels.Services.Voice.Audio
{
    public interface IAudioService
    {
        event EventHandler<float[]> AudioQueued;
        void CreateGraph(string deviceId = null);
        string DeviceId { get; }
        int Samples { get; }
        void Dispose();
    }
}
