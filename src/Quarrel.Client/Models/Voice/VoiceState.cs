// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Voice;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;

namespace Quarrel.Client.Models.Voice
{
    public class VoiceState : DiscordItem
    {
        internal VoiceState(JsonVoiceState restState, QuarrelClient context) :
            base(context)
        {
            ServerMute = restState.Mute;
            ServerDeaf = restState.Deaf;
            SelfMute = restState.SelfMute;
            SelfDeaf = restState.SelfDeaf;
            Suppress = restState.Suppress;

            var user = context.Users.GetUser(restState.UserId);
            Guard.IsNotNull(user, nameof(user));
            User = user;

            var channel = context.Channels.GetChannel(restState.ChannelId!.Value);
            Guard.IsAssignableToType<IAudioChannel>(channel!, nameof(channel));
            Channel = (IAudioChannel)channel!;
        }

        public IAudioChannel Channel { get; }

        public User User { get; }

        public bool Mute => SelfMute || ServerMute;

        public bool SelfMute { get; }

        public bool ServerMute { get; }

        public bool Deaf => SelfDeaf || ServerDeaf;

        public bool SelfDeaf { get; }

        public bool ServerDeaf { get; }

        public bool Suppress { get; }
    }
}
