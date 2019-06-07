using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Sockets
{
    public static class IWebSocketExtensions
    {
        public static async Task SendJsonObjectAsync(this IWebMessageSocket webMessageSocket, object payload)
        {
            var serialzedObject = JsonConvert.SerializeObject(payload);

            await webMessageSocket.SendMessageAsync(serialzedObject);
        }
    }
}
