using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.CacheModels
{
    public class GuildChannel
    {
        public GuildChannel(SharedModels.GuildChannel input)
        {
            Raw = input;
        }

        public GuildChannel(TempGuildChannel input)
        {
            Raw.Id = input.Id;
            Raw.GuildId = input.GuildId;
            Raw.Name = input.Name;
            Raw.Type = input.Type;
            Raw.Position = input.Position;
            Raw.Private = input.IsPrivate;
            Raw.Topic = input.Topic;
            Raw.LastMessageId = input.LastMessageId;
            Raw.PermissionOverwrites = input.Overwrites;

            foreach (TempMessage message in input.Messages)
            {
                Messages.Add(message.Id, new Message(message));
            }

            foreach (TempMessage message in input.Pinnedmessages)
            {
                PinnedMessages.Add(message.Id, new Message(message));
            }

            foreach (TempMember member in input.Members)
            {
                Members.Add(member.User.Id, new Member(member));
            }
        }

        public SharedModels.GuildChannel Raw = new SharedModels.GuildChannel();

        public Dictionary<string, Message> Messages = new Dictionary<string, Message>();
        public Dictionary<string, Message> PinnedMessages = new Dictionary<string, Message>();
        public Dictionary<string, Member> Members = new Dictionary<string, Member>();
    }
}
