// Adam Dernis © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.Services.Storage.Accounts.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quarrel.Services.Storage.Accounts
{
    /// <summary>
    /// A storage class that stores info about the user's account.
    /// </summary>
    public class AccountInfoStorage : SettingsBase, IAccountInfoStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfoStorage"/> class.
        /// </summary>
        public AccountInfoStorage(IFolderData folder, IAsyncSerializer<Stream> serializer) : base(folder, serializer)
        {
        }

        /// <summary>
        /// Gets or sets the account info in storage.
        /// </summary>
        public AccountInfo? ActiveAccount
        {
            get => GetSetting<AccountInfo?>(() => null);
            set => SetSetting(value);
        }

        /// <summary>
        /// Gets or sets the list of accounts in storage.
        /// </summary>
        public Dictionary<ulong, AccountInfo> Accounts
        {
            get => GetSetting(() => new Dictionary<ulong, AccountInfo>());
            set => SetSetting(value);
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is logged into an account.
        /// </summary>
        public bool IsLoggedIn => ActiveAccount is not null;

        public bool SelectAccount(ulong id)
        {
            if (Accounts.ContainsKey(id))
            {
                ActiveAccount = Accounts[id];
                return true;
            }

            return false;
        }

        public bool RegisterAccount(AccountInfo accountInfo)
        {
            if (!Accounts.ContainsKey(accountInfo.Id))
            {
                Accounts.Add(accountInfo.Id, accountInfo);
                return true;
            }

            return false;
        }

        public bool UnregisterAccount(ulong id)
        {
            // TODO: Handle active account 
            return Accounts.Remove(id);
        }
    }
}
