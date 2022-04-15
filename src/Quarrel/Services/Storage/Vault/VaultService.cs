// Quarrel © 2022

using Quarrel.Services.Analytics;
using Windows.Security.Credentials;

namespace Quarrel.Services.Storage.Vault
{
    public class VaultService : IVaultService
    {
        private const string TokenResource = "Token";
        private readonly PasswordVault _vault;

        public VaultService()
        {
            _vault = new PasswordVault();
        }

        public string? GetUserToken(ulong userId)
        {
            try
            {
                var credential = _vault.Retrieve(TokenResource, $"{userId}");
                credential.RetrievePassword();
                return credential.Password;
            }
            catch
            {
                // TODO: Log error
                return null;
            }
        }

        public void RegisterUserToken(ulong userId, string token)
        {
            PasswordCredential credential = new(TokenResource, $"{userId}", token);
            _vault.Add(credential);
        }

        public void UnregisterToken(ulong userId)
        {
            var credential = _vault.Retrieve(TokenResource, $"{userId}");
            _vault.Remove(credential);
        }
    }
}
