// Quarrel © 2022

using Quarrel.Services.Storage.Accounts.Models;

namespace Quarrel.Messages.Discord
{
    /// <summary>
    /// A message sent when a user is logged in to the discord service.
    /// </summary>
    public class UserLoggedInMessage
    {
        private readonly AccountInfo _accountInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoggedInMessage"/> class.
        /// </summary>
        public UserLoggedInMessage(AccountInfo accountInfo)
        {
            _accountInfo = accountInfo;
        }

        /// <summary>
        /// Gets the account info for the logged in user.
        /// </summary>
        public AccountInfo AccountInfo => _accountInfo;
    }
}
