using Quarrel.ViewModels.Messages.Abstract;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Messages.Posts.Requests
{
    /// <summary>
    /// A request message to retrieve a user currently loaded in the memberlist being displayed
    /// </summary>
    public sealed class CurrentMemberListRequestMessage : RequestMessageBase<List<BindableGuildMember>> { }
}
