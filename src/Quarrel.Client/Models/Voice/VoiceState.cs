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
        internal VoiceState(JsonVoiceState json, QuarrelClient context) :
            base(context)
        {
            var user = context.Users.GetUser(json.UserId);
            Guard.IsNotNull(user, nameof(user));
            User = user;

            ServerMute = json.Mute;
            ServerDeaf = json.Deaf;
            SelfMute = json.SelfMute;
            SelfDeaf = json.SelfDeaf;
            Suppress = json.Suppress;
            HasVideo = json.SelfVideo;
            IsStreaming = json.SelfStream ?? false;

            GetChannel(json.ChannelId);
        }

        /// <summary>
        /// Gets the channel the user is in.
        /// </summary>
        public IAudioChannel? Channel { get; private set; }

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
        public bool SelfMute { get; private set;}

        /// <summary>
        /// Gets whether or not the user has been muted by someone else.
        /// </summary>
        public bool ServerMute { get; private set; }

        /// <summary>
        /// Gets whether or not the user is deafened.
        /// </summary>
        public bool Deaf => SelfDeaf || ServerDeaf;

        /// <summary>
        /// Gets whether or not the user has deafened themselves.
        /// </summary>
        public bool SelfDeaf { get; private set;}

        /// <summary>
        /// Gets whether or not the user has been deafened by someone else.
        /// </summary>
        public bool ServerDeaf { get; private set;}

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        public bool Suppress { get; private set;}
        
        /// <summary>
        /// Gets whether or not the user has their camera turned on.
        /// </summary>
        public bool HasVideo { get; private set; }
        
        /// <summary>
        /// Gets whether or not the user is streaming their screen.
        /// </summary>
        public bool IsStreaming { get; private set; }

        internal void Update(JsonVoiceState json)
        {
            Guard.IsEqualTo(json.UserId, User.Id, nameof(json.UserId));
            
            ServerMute = json.Mute;
            ServerDeaf = json.Deaf;
            SelfMute = json.SelfMute;
            SelfDeaf = json.SelfDeaf;
            Suppress = json.Suppress;
            HasVideo = json.SelfVideo;
            IsStreaming = json.SelfStream ?? false;

            GetChannel(json.ChannelId);
        }

        private void GetChannel(ulong? channelId)
        {
            if (channelId.HasValue)
            {
                var channel = Context.Channels.GetChannel(channelId.Value);
                Guard.IsAssignableToType<IAudioChannel>(channel!, nameof(channel));
                Channel = (IAudioChannel)channel!;
            }
        }
    }
}
