using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordPipeImpersonator
{
    class PipeFrameTypes
    {
        internal class Handshake
        {
            /// <summary>
            /// Version of the IPC API we are using
            /// </summary>
            [JsonProperty("v")]
            public int Version { get; set; }

            /// <summary>
            /// The ID of the app.
            /// </summary>
            [JsonProperty("client_id")]
            public string ClientID { get; set; }
        }
    }
}
