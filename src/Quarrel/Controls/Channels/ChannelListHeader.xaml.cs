using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Channels
{
    public sealed partial class ChannelListHeader : UserControl
    {
        public ChannelListHeader()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();

                if (ViewModel != null)
                {
                    if (ViewModel.Model.BannerUri == null)
                        rootButton.Height = 48;
                    else
                        rootButton.Height = 64;
                }
            };
        }

        public BindableGuild ViewModel => DataContext as BindableGuild;

        private async void ImageEx_ImageExOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            await Banner.Fade(1, 200).StartAsync();
        }
    }
}
