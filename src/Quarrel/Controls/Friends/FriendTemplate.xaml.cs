using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Friends
{
    /// <summary>
    /// Template shown in FriendListControl for Friend items
    /// </summary>
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

        /// <summary>
        /// Friend displayed as bindable object
        /// </summary>
        public BindableFriend ViewModel => DataContext as BindableFriend;
    }
}
