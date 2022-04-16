// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Client.Models.Users;

namespace Quarrel.Bindables.Users
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Users.SelfUser"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableSelfUser : ObservableObject
    {
        [ObservableProperty]
        private SelfUser _selfUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableSelfUser"/> class.
        /// </summary>
        internal BindableSelfUser(SelfUser selfUser)
        {
            _selfUser = selfUser;
        }
    }
}
