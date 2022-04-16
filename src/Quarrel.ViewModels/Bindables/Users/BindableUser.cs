// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Client.Models.Users;

namespace Quarrel.Bindables.Users
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Users.User"/> that can be bound to the UI.
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
