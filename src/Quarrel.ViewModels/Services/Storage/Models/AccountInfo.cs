// Adam Dernis © 2022

namespace Quarrel.ViewModels.Services.Storage.Models
{
    /// <summary>
    /// A model containing 
    /// </summary>
    public class AccountInfo
    {
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
        /// Gets or sets the last access token for the account.
        /// </summary>
        public string? LastAccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for the account.
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
