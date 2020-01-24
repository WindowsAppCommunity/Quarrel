using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    public sealed partial class TopicPage : UserControl, IConstrainedSubPage
    {
        private ISubFrameNavigationService subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        public TopicPage()
        {
            this.InitializeComponent();

            if (subFrameNavigationService.Parameter != null)
            {
                this.DataContext = subFrameNavigationService.Parameter;
            }
        }

        public BindableChannel ViewModel => DataContext as BindableChannel;

        public double MaxExpandedHeight => 200;
        public double MaxExpandedWidth => 800;
    }
}
