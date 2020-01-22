using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableUser : BindableModelBase<User>, IEquatable<BindableUser>, IComparable<BindableUser>
    {
        #region Constructors

        public BindableUser([NotNull] User model) : base(model) { }

        #endregion

        #region Properties

        private Presence presence;
        public Presence Presence
        {
            get => presence;
            set => Set(ref presence, value);
        }

        #endregion

        #region Interfaces
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

        #endregion
    }
}
