// Adam Dernis © 2022

namespace Quarrel.Services.Storage.Accounts.Models
{
    public class AccountInfo
    {
        public AccountInfo(string token)
        {
            Token = token;
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the username for the account.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the discriminator for the account.
        /// </summary>
        public int? Discriminator { get; set; }

        /// <summary>
        /// Gets or sets the token for the account.
        /// </summary>
        public string Token { get; }
    }
}
