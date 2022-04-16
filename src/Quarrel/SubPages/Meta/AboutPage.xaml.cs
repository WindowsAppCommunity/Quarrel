// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Meta;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Meta
{
    public sealed partial class AboutPage : UserControl
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        AboutPageViewModel ViewModel => (AboutPageViewModel)DataContext;
    }
}
