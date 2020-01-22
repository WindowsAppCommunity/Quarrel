using DiscordAPI.Models;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using System;

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
