using System.Collections.Generic;
using DiscordAPI.Models;
using Quarrel.Messages.Abstract;
using Quarrel.Models.Bindables;

namespace Quarrel.Messages.Posts.Requests
{
    public sealed class CurrentUserVoiceStateRequestMessage : RequestMessageBase<VoiceState> { }
}
