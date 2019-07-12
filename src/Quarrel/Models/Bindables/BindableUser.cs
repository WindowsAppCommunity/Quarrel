using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.Messages.Gateway;
using Quarrel.Models.Bindables.Abstract;
using UICompositionAnimations.Helpers;

namespace Quarrel.Models.Bindables
{
    public class BindableUser : BindableModelBase<User>, IEquatable<BindableUser>, IComparable<BindableUser>
    {

        public BindableUser([NotNull] User model) : base(model)
        {

        }

        public bool Equals(BindableUser other)
        {
            return Model.Id == other.Model.Id;
        }

        public int CompareTo(BindableUser other)
        {
            return Model.Id.CompareTo(other.Model.Id);
        }

        public override int GetHashCode()
        {
            return Model.Id.GetHashCode();
        }

        public Presence presence;

        public Presence Presence
        {
            get => presence;
            set => Set(ref presence, value);
        }
    }
}
