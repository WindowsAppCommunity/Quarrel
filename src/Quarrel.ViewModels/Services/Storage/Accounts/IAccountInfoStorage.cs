// Quarrel © 2022

using Quarrel.Services.Storage.Accounts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Services.Storage.Accounts
{
    /// <summary>
    /// An interface for a service that that stores info about the user's account.
    /// </summary>
    public interface IAccountInfoStorage
    {
        /// <summary>
        /// Gets the account info in storage.
        /// </summary>
        AccountInfo? ActiveAccount { get; }
        /// <summary>
        /// Gets a list of accounts
        /// </summary>
        IReadOnlyDictionary<ulong, AccountInfo> Accounts { get; }

        /// <summary>
        /// Changes the active account.
        /// </summary>
        /// <param name="id">The if of the account to select.</param>
        /// <returns>True if the account was selected. False otherwise.</returns>
        bool SelectAccount(ulong id);
        
        /// <summary>
        /// Adds a new account to the account registry.
        /// </summary>
        /// <param name="accountInfo">The account to register.</param>
        /// <returns>False if the account was already registered. True otherwise.</returns>
        bool RegisterAccount(AccountInfo accountInfo);
        
        /// <summary>
        /// Removes an account from the registered account list.
        /// </summary>
        /// <param name="id">The id of the account to remove.</param>
        /// <returns>True if the account was found and removed. False otherwise.</returns>
        bool UnregisterAccount(ulong id);

        /// <summary>
        /// Loads settings from memory.
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// Saves settings to memory.
        /// </summary>
        Task SaveAsync();
    }
}
