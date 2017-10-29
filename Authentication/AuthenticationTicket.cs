using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Authentication
{
    public struct AuthenticationTicket
    {
        public string Type { get; set; }
        public string Token { get; set; }
    }
}
