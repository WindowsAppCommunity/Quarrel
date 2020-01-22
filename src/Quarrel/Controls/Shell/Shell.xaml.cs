using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quarrel.Messages.Navigation;
using Quarrel.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class Shell : UserControl
    {
        //private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
       // private ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        public Shell()
        {
            try
            {
                this.InitializeComponent();

                // Setup SideDrawer
                ContentContainer.SetupInteraction();

                Messenger.Default.Register<GuildNavigateMessage>(this, m =>
                {
                    ContentContainer.OpenLeft();
                });

                Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
                {
                    ContentContainer.CloseLeft();
                });
            }
            catch (Exception ex)
            {
                var logger = App.ServiceProvider.GetService<ILogger<Shell>>();

                do
                {
                    logger.LogError(new EventId(), ex, "Error constructing Shell");
                    ex = ex.InnerException;
                } while (ex != null);

                throw;
            }
        }
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        private bool IsViewLarge => UISize.CurrentState == Large || UISize.CurrentState == ExtraLarge;

        private void UISize_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            this.Bindings.Update();
        }

        private void HamburgerClicked(object sender, EventArgs e)
        {
            ContentContainer.ToggleLeft();
        }

        private void MemberListButtonClicked(object sender, EventArgs e)
        {
            ContentContainer.ToggleRight();
        }
    }
}
