// Quarrel © 2022

using System;

namespace Quarrel.Services.Storage.Vault
{
    /// <summary>
    /// An interface for a service that stores items in a secure vault.
    /// </summary>
    public interface IVaultService
    {
        /// <summary>
        /// Gets a the token for a user from the password vault
        /// </summary>
        /// <param name="userId">The id of the user to get the token for.</param>
        /// <exception cref="Exception">An exception will be thrown if the resource does not exist.</exception>
        /// <returns>The id of the user to get the token for.</returns>
        string? GetUserToken(ulong userId);

        /// <summary>
        /// Registers a token in the vault by user id.
        /// </summary>
        /// <param name="userId">The id of the user the token belongs to.</param>
        /// <param name="token">The token to store.</param>
        void RegisterUserToken(ulong userId, string token);
        
        /// <summary>
        /// Unregisters a token in the vault by user id.
        /// </summary>
        /// <param name="userId">The id of the user the token belongs to.</param>
        void UnregisterToken(ulong userId);
    }
}
