using Quarrel.ViewModels.Models.Bindables.GitHub;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.GitHub
{
    public sealed partial class DeveloperTemplate : UserControl
    {
        public DeveloperTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        public BindableDeveloper ViewModel => DataContext as BindableDeveloper;
    }
}
