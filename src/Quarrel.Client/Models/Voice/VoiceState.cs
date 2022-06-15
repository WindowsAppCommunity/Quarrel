// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Voice;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;

namespace Quarrel.Client.Models.Voice
{
    /// <summary>
    /// A model declaring the state of a user's voice.
    /// </summary>
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

            if (restState.ChannelId.HasValue)
            {
                var channel = context.Channels.GetChannel(restState.ChannelId.Value);
                Guard.IsAssignableToType<IAudioChannel>(channel!, nameof(channel));
                Channel = (IAudioChannel)channel!;
            }
        }

        /// <summary>
        /// Gets the channel the user is in.
        /// </summary>
        public IAudioChannel? Channel { get; }

        /// <summary>
        /// Gets the user who's voice state is being defined.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets whether or not the user is muted.
        /// </summary>
        public bool Mute => SelfMute || ServerMute;

        /// <summary>
        /// Gets whether or not the user has muted themselves.
        /// </summary>
        public bool SelfMute { get; }

        /// <summary>
        /// Gets whether or not the user has been muted by someone else.
        /// </summary>
        public bool ServerMute { get; }

        /// <summary>
        /// Gets whether or not the user is deafened.
        /// </summary>
        public bool Deaf => SelfDeaf || ServerDeaf;

        /// <summary>
        /// Gets whether or not the user has deafened themselves.
        /// </summary>
        public bool SelfDeaf { get; }

        /// <summary>
        /// Gets whether or not the user has been deafened by someone else.
        /// </summary>
        public bool ServerDeaf { get; }

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        public bool Suppress { get; }
    }
}
