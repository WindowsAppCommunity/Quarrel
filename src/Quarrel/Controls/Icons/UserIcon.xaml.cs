// Quarrel © 2022

using Quarrel.Bindables.Users.Interfaces;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Icons
{
    public sealed partial class UserIcon : UserControl
    {
        public UserIcon()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        public IBindableUser ViewModel => (IBindableUser)DataContext;
    }
}
