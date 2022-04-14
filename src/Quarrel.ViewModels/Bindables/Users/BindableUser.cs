// Adam Dernis © 2022

using Discord.API.Models.Users;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Quarrel.Bindables.Users
{
    /// <summary>
    /// A wrapper of a <see cref="Discord.API.Models.Users.User"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableUser : ObservableObject
    {
        [ObservableProperty]
        private User _user;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableUser"/> class.
        /// </summary>
        internal BindableUser(User user)
        {
            _user = user;
        }
    }
}
