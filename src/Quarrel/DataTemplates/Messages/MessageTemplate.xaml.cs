using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Controls.Members;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Navigation;
using System;
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
                var guildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
                BindableGuildMember member = guildsService.GetGuildMember(e.User.Id, guildsService.CurrentGuild.Model.Id);
                if (member != null)
                {
                    Flyout flyout = new Flyout()
                    {
                        Content = new MemberFlyoutTemplate() { DataContext = member },
                        FlyoutPresenterStyle = App.Current.Resources["GenericFlyoutStyle"] as Style
                    };
                    flyout.ShowAt(sender as FrameworkElement);
                }
            }
            else if (e.Channel != null)
            {
                Messenger.Default.Send(new ChannelNavigateMessage(e.Channel, e.Channel.Guild));
            }
            else
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
