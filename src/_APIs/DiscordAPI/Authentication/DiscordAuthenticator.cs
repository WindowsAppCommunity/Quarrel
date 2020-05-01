namespace DiscordAPI.Authentication
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
