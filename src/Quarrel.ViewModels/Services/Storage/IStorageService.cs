// Quarrel © 2022

using Quarrel.Services.Storage.Accounts;

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
    }
}
