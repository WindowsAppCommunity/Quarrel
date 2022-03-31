// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Users;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels
{
    public class DirectChannel : Channel, IDirectChannel
    {
        internal DirectChannel(JsonChannel restChannel) : base(restChannel)
        {            
            Guard.IsNotNull(restChannel.Recipient, nameof(restChannel.Recipient));

            Recipient = new User(restChannel.Recipient);
        }

        public User Recipient { get; private set; }

        IUser IDirectChannel.Recipient => Recipient;

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Recipient = Recipient.ToRestUser();
            return restChannel;
        }

    }
}
