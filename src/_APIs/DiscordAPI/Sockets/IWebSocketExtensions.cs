// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DiscordAPI.Sockets
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
