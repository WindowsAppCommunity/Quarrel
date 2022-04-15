// Quarrel © 2022

using Quarrel.Services.Storage.Accounts;
using Quarrel.Services.Storage.Vault;

namespace Quarrel.Services.Storage
{
    /// <summary>
    /// An interface for a service that contains other storage services.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// A service for account storage.
        /// </summary>
        IAccountInfoStorage AccountInfoStorage { get; }

        /// <summary>
        /// A service for storing tokens in a vault.
        /// </summary>
        IVaultService VaultService { get; }
    }
}
