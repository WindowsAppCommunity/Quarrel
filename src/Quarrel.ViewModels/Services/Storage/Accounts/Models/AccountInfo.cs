// Quarrel © 2022

namespace Quarrel.Services.Storage.Accounts.Models
{
    /// <summary>
    /// A class containing information about a user for the sake of login.
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfo"/> class.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="discriminator">The discriminator of the user.</param>
        /// <param name="token">A login token for the user.</param>
        public AccountInfo(ulong id, string username, int discriminator, string token)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            Token = token;
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Gets or sets the username for the account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the discriminator for the account.
        /// </summary>
        public int Discriminator { get; set; }

        /// <summary>
        /// Gets or sets the token for the account.
        /// </summary>
        public string Token { get; }
    }
}
