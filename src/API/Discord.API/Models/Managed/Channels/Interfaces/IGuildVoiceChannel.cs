// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IGuildVoiceChannel : INestedChannel, IAudioChannel
    {
        int Bitrate { get; }

        int? UserLimit { get; }
    }
}
