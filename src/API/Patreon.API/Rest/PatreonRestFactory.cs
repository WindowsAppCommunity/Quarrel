// Quarrel © 2022

namespace Patreon.API.Rest
{
    public class PatreonRestFactory
    {
        private const string BaseUrl = "https://www.patreon.com/api/oauth2/v2/";
        private readonly string _token;

        public PatreonRestFactory(string token)
        {
            _token = token;
        }


    }
}
