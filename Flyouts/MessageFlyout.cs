using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Discord_UWP.Managers;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeMessageMenu(SharedModels.Message message)
        {

            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];


            MenuFlyoutItem reply = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/ReplyItem"),
                Tag = message,
                Icon = new SymbolIcon(Symbol.MailReply)
            };
            //reply.Click += FlyoutManager.;


            MenuFlyoutItem pin = new MenuFlyoutItem()
            {
                Text = message.Pinned ? App.GetString("/Controls/Unpin") : App.GetString("/Controls/Pin"),
                Tag = message.Id,
                Icon = new SymbolIcon(Symbol.Pin)
            };
            //pin.Click += FlyoutManager.;


            MenuFlyoutItem addReaction = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/AddReacItem"),
                Tag = message.Id,
                Icon = new SymbolIcon(Symbol.Emoji)
            };
            //addReaction.Click += FlyoutManager.;


            MenuFlyoutItem edit = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/EditItem"),
                Tag = message,
                Icon = new SymbolIcon(Symbol.Edit)
            };
            //edit.Click += FlyoutManager.;

            
            MenuFlyoutItem delete = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/DeleteItem"),
                Tag = member.User.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.Delete)
            };
            //delete.Click += FlyoutManager.;


            if (LocalState.CurrentUser.Id == message.User.Id)
            {
                menu.Items.Add(edit);
            }


            if (App.CurrentGuildId != null)
            {
                if (LocalState.Guilds[App.CurrentGuildId].channels[message.ChannelId].permissions.ManageMessages || LocalState.Guilds[App.CurrentGuildId].channels[message.ChannelId].permissions.Administrator || message?.User.Id == LocalState.CurrentUser.Id || LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId == LocalState.CurrentUser.Id)
                {
                    menu.Items.Add(delete);
                }
            }


            if (Storage.Settings.DevMode)
            {
                MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
                menu.Items.Add(sep1);

                MenuFlyoutItem copyId = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Controls/CopyIdItem"),
                    Tag = message.User.Id,
                    //Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Copy)
                };
                //copyId.Click += FlyoutManager.;
            }


            return menu;
        }
    }
}
