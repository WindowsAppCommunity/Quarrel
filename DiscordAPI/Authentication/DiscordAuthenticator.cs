using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Authentication
{
    public class DiscordAuthenticator : IAuthenticator
    {
        public string Token { get; set; }

        public DiscordAuthenticator(string token)
        {
            Token = token;
        }

        public string GetToken()
        {
            return Token; 
        }
    }
}
