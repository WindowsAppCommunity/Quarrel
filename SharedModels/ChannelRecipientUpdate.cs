using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public class ChannelRecipientUpdate
    {
        public User user { get; set; }
        public string channel_id { get; set; }
    }
}
