using DiscordAPI.Models;
using Quarrel.ViewModels.Messages.Abstract;

namespace Quarrel.ViewModels.Messages.Posts.Requests
{
    /// <summary>
    /// A request message to retrieve a user currently loaded in the memberlist being displayed
    /// </summary>
    public sealed class PresenceRequestMessage : RequestMessageBase<Presence>
    {
        public PresenceRequestMessage(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
