// Adam Dernis © 2022

using Quarrel.Services.Storage.Accounts.Models;
using System.Threading.Tasks;

namespace Quarrel.Services.Storage.Accounts
{
    public interface IAccountInfoStorage
    {
        /// <summary>
        /// Gets or sets the account info in storage.
        /// </summary>
        AccountInfo? ActiveAccount { get; }

        /// <summary>
        /// Gets a value indicating whether or not the user is logged into an account.
        /// </summary>
        bool IsLoggedIn { get; }

        bool SelectAccount(ulong id);

        bool RegisterAccount(AccountInfo accountInfo);

        bool UnregisterAccount(ulong id);

        Task LoadAsync();

        Task SaveAsync();
    }
}
