using GalaSoft.MvvmLight.Ioc;
using Quarrel.Controls.Members;
using Quarrel.Models.Bindables;
using Quarrel.Navigation;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.DataTemplates.Messages
{
    public partial class MessageTemplate
    {
        public MessageTemplate()
        {
            this.InitializeComponent();
        }

        private void Expand(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var attachment = (e.OriginalSource as FrameworkElement).DataContext;
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("AttachmentPage", attachment);
        }

        private async void Markdown_LinkClicked(object sender, Controls.Markdown.LinkClickedEventArgs e)
        {
            if (e.User != null)
            {
                BindableGuildMember member;
                SimpleIoc.Default.GetInstance<ICurrentUsersService>().Users.TryGetValue(e.User.Id, out member);
                if (member != null)
                {
                    Flyout flyout = new Flyout()
                    {
                        Content = new MemberFlyoutTemplate() { DataContext = member },
                        FlyoutPresenterStyle = App.Current.Resources["GenericFlyoutStyle"] as Style
                    };
                    flyout.ShowAt(sender as FrameworkElement);
                }
            } else
            {
                Uri uri;
                if (Uri.TryCreate(e.Link, UriKind.Absolute, out uri))
                {
                    await Launcher.LaunchUriAsync(uri);
                }
            }
        }
    }
}
