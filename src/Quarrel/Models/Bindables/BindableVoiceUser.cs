using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Services.Users;

namespace Quarrel.Models.Bindables
{
    public class BindableVoiceUser : BindableModelBase<VoiceState>, IEquatable<BindableVoiceUser>, IComparable<BindableVoiceUser>
    {
        public ICurrentUsersService UserService => SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        public BindableUser User => UserService?.Users[Model.UserId]?[Model.UserId];

        public bool ShowDeaf => Model.SelfDeaf || Model.ServerDeaf;
        public bool ServerDeaf => ShowDeaf && Model.ServerDeaf;
        public bool LocalDeaf => ShowDeaf && !Model.ServerDeaf;

        public bool ShowMute => Model.SelfMute || Model.ServerMute;
        public bool ServerMute => ShowMute && Model.ServerMute;
        public bool LocalMute => ShowMute && !Model.ServerMute;

        public BindableVoiceUser([NotNull] VoiceState model) : base(model)
        {
        }
        public bool Equals(BindableVoiceUser other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(BindableVoiceUser other)
        {
            throw new NotImplementedException();
        }
    }
}
