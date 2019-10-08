using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Quarrel.Messages.Navigation.SubFrame;
using Quarrel.Services;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Navigation;
using Quarrel.SubPages;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;
using Quarrel.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


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
