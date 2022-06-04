// Quarrel © 2022

namespace Quarrel.Services.Storage.Accounts.Models
{
    /// <summary>
    /// A class containing information about a user for the sake of login.
    /// </summary>
    /// <param name="Id">The ID of the user.</param>
    /// <param name="Username">The username of the user.</param>
    /// <param name="Discriminator">The discriminator of the user.</param>
    /// <param name="Token">A login token for the user.</param>
    public record AccountInfo(ulong Id, string Username, int Discriminator, string Token);
}
