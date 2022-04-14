// Quarrel © 2022

using Discord.API.Models.Users;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Quarrel.Bindables.Users
{
    /// <summary>
    /// A wrapper of a <see cref="Discord.API.Models.Users.SelfUser"/> that can be bound to the UI.
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
