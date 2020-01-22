using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Friends
{
    public sealed partial class FriendTemplate : UserControl
    {
        public FriendTemplate()
        {
            this.InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        public BindableFriend ViewModel => DataContext as BindableFriend;
    }
}
