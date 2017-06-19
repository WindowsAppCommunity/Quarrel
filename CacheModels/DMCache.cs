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
            Raw = input.Raw;
            foreach (TempMessage message in input.Messages)
            {
                Messages.Add(message.Id, new Message(message));
            }
        }

        public DmCache(SharedModels.DirectMessageChannel input)
        {
            Raw = input;
        }


        public SharedModels.DirectMessageChannel Raw = new SharedModels.DirectMessageChannel();

        public Dictionary<string, Message> Messages = new Dictionary<string, Message>();
    }
}
