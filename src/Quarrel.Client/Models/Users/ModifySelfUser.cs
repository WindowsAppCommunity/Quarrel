// Quarrel © 2022

using Discord.API.Models.Json.Users;

namespace Quarrel.Client.Models.Users
{
    /// <summary>
    /// An object for modifying the self user.
    /// </summary>
    public class ModifySelfUser
    {
        /// <summary>
        /// Gets or sets the user's bio.
        /// </summary>
        public string? AboutMe { get; set; }

        internal JsonModifySelfUser ToJsonModel()
        {
            return new JsonModifySelfUser()
            {
                AboutMe = AboutMe,
            };
        }
    }
}
