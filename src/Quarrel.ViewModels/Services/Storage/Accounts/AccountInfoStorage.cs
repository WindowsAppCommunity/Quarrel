// Quarrel © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.Services.Storage.Accounts.Models;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfoStorage"/> class.
        /// </summary>
        public AccountInfoStorage(IFolderData folder, IAsyncSerializer<Stream> serializer) : base(folder, serializer)
        {
        }

        /// <inheritdoc/>
        public AccountInfo? ActiveAccount
        {
            get => ActiveAccountId.HasValue ? Accounts[ActiveAccountId.Value] : null;
            set => ActiveAccountId = value is not null ? value.Id : null;
        }

        /// <summary>
        /// Gets or sets the list of accounts in storage.
        /// </summary>
        public Dictionary<ulong, AccountInfo> Accounts
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
                return true;
            }

            return false;
        }
        
        /// <inheritdoc/>
        public bool UnregisterAccount(ulong id)
        {
            // TODO: Handle active account 
            return Accounts.Remove(id);
        }
        
        /// <inheritdoc/>
        Task IAccountInfoStorage.LoadAsync() => LoadAsync();
        
        /// <inheritdoc/>
        Task IAccountInfoStorage.SaveAsync() => SaveAsync();
    }
}
   