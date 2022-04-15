// Quarrel © 2022

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

        /// <inheritdoc/>
        public string GetUserToken(ulong userId)
        {
            var credential = _vault.Retrieve(TokenResource, $"{userId}");
            credential.RetrievePassword();
            return credential.Password;
        }
        
        /// <inheritdoc/>
        public void RegisterUserToken(ulong userId, string token)
        {
            PasswordCredential credential = new(TokenResource, $"{userId}", token);
            _vault.Add(credential);
        }
        
        /// <inheritdoc/>
        public void UnregisterToken(ulong userId)
        {
            var credential = _vault.Retrieve(TokenResource, $"{userId}");
            _vault.Remove(credential);
        }
    }
}
