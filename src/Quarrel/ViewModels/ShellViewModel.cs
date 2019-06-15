// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        /// <summary>
        /// Raised whenever the local user changes
        /// </summary>
        public event EventHandler LoggedUserChanged;

        private User _CurrentUser;

        /// <summary>
        /// Get the currently logged in user if present
        /// </summary>
        [NotNull]
        internal User CurrentUser
        {
            get => _CurrentUser;
            private set
            {
                if (Set(ref _CurrentUser, value))
                    LoggedUserChanged?.Invoke(this, null);
            }
        }
    }
}
