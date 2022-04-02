// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface IGuildVoiceChannel : INestedChannel, IAudioChannel
    {
        int Bitrate { get; }

        int? UserLimit { get; }
    }
}
