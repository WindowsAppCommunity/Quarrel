using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.CacheModels
{
    public class DmCache
    {
        public DmCache(TempDmCache input)
        {
            Raw.Id = input.Id;
            Raw.LastMessageId = input.LastMessageId;
            Raw.Private = input.Private;
            Raw.Type = input.Type;
            //Raw.User = input.User;
            Raw.Users = input.Users.AsEnumerable();
            foreach (TempMessage message in input.Messages)
            {
                Messages.Add(message.Id, new Message(message));
            }

            foreach (TempMessage message in input.PinnedMessages)
            {
                PinnedMessages.Add(message.Id, new Message(message));
            }
        }

        public DmCache(SharedModels.DirectMessageChannel input)
        {
            Raw = input;
        }


        public SharedModels.DirectMessageChannel Raw = new SharedModels.DirectMessageChannel();
        public Dictionary<string, Message> Messages = new Dictionary<string, Message>();
        public Dictionary<string, Message> PinnedMessages = new Dictionary<string, Message>();
    }
}
