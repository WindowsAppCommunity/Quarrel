using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayNoteUpdatedMessage
    {
        public GatewayNoteUpdatedMessage(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
