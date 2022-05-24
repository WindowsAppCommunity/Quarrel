// Quarrel © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.Services.Storage.Accounts.Models;
using Quarrel.Services.Storage.Vault;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Quarrel.Services.Storage.Accounts
{
    /// <summary>
    /// A service that stores info about the user's account.
    /// </summary>
    public class AccountInfoStorage : SettingsBase, IAccountInfoStorage
    {
        private readonly IVaultService _vaultService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfoStorage"/> class.
        /// </summary>
        public AccountInfoStorage(IVaultService vaultService, IFolderData folder, IAsyncSerializer<Stream> serializer) : base(folder, serializer)
        {
            _vaultService = vaultService;
        }

        /// <inheritdoc/>
        public AccountInfo? ActiveAccount
        {
            get => ActiveAccountId.HasValue ? GetPopulatedAccountInfo(ActiveAccountId.Value) : null;
            set => ActiveAccountId = value?.Id;
        }

        /// <summary>
        /// Gets or sets the list of accounts in storage.
        /// </summary>
        private Dictionary<ulong, AccountInfo> Accounts
        {
            get => GetSetting(() => new Dictionary<ulong, AccountInfo>());
            set => SetSetting(value);
        }

        /// <inheritdoc/>
        public bool IsLoggedIn => ActiveAccountId is not null;

        private ulong? ActiveAccountId
        {
            get => GetSetting<ulong?>(() => null);
            set => SetSetting(value);
        }
        
        /// <inheritdoc/>
        public bool SelectAccount(ulong id)
        {
            if (Accounts.ContainsKey(id))
            {
                ActiveAccountId = id;
                return true;
            }

            return false;
        }
        
        /// <inheritdoc/>
        public bool RegisterAccount(AccountInfo accountInfo)
        {
            if (!Accounts.ContainsKey(accountInfo.Id))
            {
                Accounts.Add(accountInfo.Id, accountInfo);
                _vaultService.RegisterUserToken(accountInfo.Id, accountInfo.Token);
                return true;
            }

            return false;
        }
        
        /// <inheritdoc/>
        public bool UnregisterAccount(ulong id)
        {
            // TODO: Handle active account 
            _vaultService.UnregisterToken(id);
            return Accounts.Remove(id);
        }

        private AccountInfo? GetPopulatedAccountInfo(ulong id)
        {
            // Get the account info from storage
            // Ensure the account is actually registered 
            AccountInfo accountInfo = Accounts[id];
            if (accountInfo is null) return null;

            // Get the token from the vault
            // Ensure the token was in the vault
            string? token = _vaultService.GetUserToken(id);
            if (token is null) return null;
            accountInfo.Token = token;

            // Return the populated account info.
            return accountInfo;
        }
        
        /// <inheritdoc/>
        Task IAccountInfoStorage.LoadAsync() => LoadAsync();
        
        /// <inheritdoc/>
        Task IAccountInfoStorage.SaveAsync() => SaveAsync();
    }
}
