using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway.DownstreamEvents
{
    public struct SessionReplace
    {
        public string status { get; set; }
        public string session_id { get; set; }
        public object game { get; set; }
        public ClientInfo client_info { get; set; }

        public struct ClientInfo
        {
            public int version { get; set; }
            public string os { get; set; }
            public string client { get; set; }
        }
    }
}
