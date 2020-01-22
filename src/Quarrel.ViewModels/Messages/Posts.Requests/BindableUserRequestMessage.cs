using Quarrel.ViewModels.Messages.Abstract;
using Quarrel.ViewModels.Models.Bindables;

namespace Quarrel.ViewModels.Messages.Posts.Requests
{
    /// <summary>
    /// A request message to retrieve a user currently loaded in the memberlist being displayed
    /// </summary>
    public sealed class BindableGuildMemberRequestMessage : RequestMessageBase<BindableGuildMember>
    {
        public BindableGuildMemberRequestMessage(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
