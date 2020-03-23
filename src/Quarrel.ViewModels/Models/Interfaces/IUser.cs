using DiscordAPI.Models;

namespace Quarrel.ViewModels.Models.Interfaces
{
    /// <summary>
    /// An interface for all bindable user objects.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets the raw <see cref="User"/> type for the user.
        /// </summary>
        User RawModel { get; }
    }
}
