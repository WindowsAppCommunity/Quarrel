using DiscordAPI.Models;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableFriend : BindableModelBase<Friend>
    {
        #region Constructors
        public BindableFriend([NotNull] Friend friend) : base(friend) { }
        
        #endregion

        #region Properties

        #region FriendType

        public bool NotFriend => Model.Type == 0;

        public bool IsFriend => Model.Type == 1;

        public bool IsBlocked => Model.Type == 2;

        public bool IsIncoming => Model.Type == 3;

        public bool IsOutgoing => Model.Type == 4;

        #endregion

        #endregion
    }
}
