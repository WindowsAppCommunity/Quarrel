// Quarrel © 2022

namespace Patreon.API.Rest
{
    /// <summary>
    /// A class for initializing Patreon rest services.
    /// </summary>
    public class PatreonRestFactory
    {
        private const string BaseUrl = "https://www.patreon.com/api/oauth2/v2/";
        private readonly string _token;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PatreonRestFactory"/> class.
        /// </summary>
        public PatreonRestFactory(string token)
        {
            _token = token;
        }
    }
}
