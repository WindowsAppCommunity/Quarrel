// Quarrel © 2022

using Quarrel.Services.Storage.Accounts.Models;

namespace Quarrel.Messages.Discord
{
    public class UserLoggedInMessage
    {
        private AccountInfo _accountInfo;

        public UserLoggedInMessage(AccountInfo accountInfo)
        {
            _accountInfo = accountInfo;
        }

        public AccountInfo AccountInfo => _accountInfo;
    }
}
