using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class ChannelRecipientUpdate
    {
        public User user { get; set; }
        public string channel_id { get; set; }
    }
}
