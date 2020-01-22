using Quarrel.ViewModels.Messages.Abstract;
using Quarrel.ViewModels.Models.Bindables;

namespace Quarrel.ViewModels.Messages.Posts.Requests
{
    /// <summary>
    /// A request message to retrieve a user currently loaded in the memberlist being displayed
    /// </summary>
    public sealed class BindableChannelRequestMessage : RequestMessageBase<BindableChannel>
    {
        public BindableChannelRequestMessage(string channelId)
        {
            ChannelId = channelId;
        }

        public string ChannelId { get; }
    }
}
