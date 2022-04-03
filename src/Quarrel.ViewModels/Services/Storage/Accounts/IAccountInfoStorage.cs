// Adam Dernis © 2022

using Quarrel.Services.Storage.Accounts.Models;

namespace Quarrel.Services.Storage.Accounts
{
    public interface IAccountInfoStorage
    {
        bool IsLoggedIn { get; }

        bool SelectAccount(ulong id);

        bool RegisterAccount(AccountInfo accountInfo);

        bool UnregisterAccount(ulong id);
    }
}
