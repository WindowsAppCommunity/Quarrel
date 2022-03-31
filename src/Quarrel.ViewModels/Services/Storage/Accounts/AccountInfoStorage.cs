// Adam Dernis © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.Services.Storage.Accounts.Models;
using System.IO;

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
        internal AccountInfoStorage(IFolderData folder, IAsyncSerializer<Stream> serializer) : base(folder, serializer)
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
        public AccountInfo[] Accounts
        {
            get => GetSetting(() => new AccountInfo[0]);
            set => SetSetting(value);
        }
    }
}
