// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Meta;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Meta
{
    public sealed partial class CreditPage : UserControl
    {
        public CreditPage()
        {
            this.InitializeComponent();
        }

        CreditPageViewModel ViewModel => (CreditPageViewModel)DataContext;
    }
}
