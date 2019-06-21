using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Quarrel.Managers;
using DiscordAPI.SharedModels;
using Quarrel.LocalModels;

namespace Quarrel.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeMessageMenu(Message message)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Create "Reply" button
            MenuFlyoutItem reply = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/ReplyItem"),
                Tag = message,
                Icon = new SymbolIcon(Symbol.MailReply)
            };
            //reply.Click += FlyoutManager.;

            // Create "Pin" button
            MenuFlyoutItem pin = new MenuFlyoutItem()
            {
                Text = message.Pinned ? App.GetString("/Controls/Unpin") : App.GetString("/Controls/Pin"),
                Tag = message.Id,
                Icon = new SymbolIcon(Symbol.Pin)
            };
            //pin.Click += FlyoutManager.;

            // Create "Add Reaction" button
            MenuFlyoutItem addReaction = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/AddReacItem"),
                Tag = message.Id,
                Icon = new SymbolIcon(Symbol.Emoji)
            };
            //addReaction.Click += FlyoutManager.;

            // Create "Edit" button
            MenuFlyoutItem edit = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/EditItem"),
                Tag = message,
                Icon = new SymbolIcon(Symbol.Edit)
            };
            //edit.Click += FlyoutManager.;

            // Create "Delete" button
            MenuFlyoutItem delete = new MenuFlyoutItem()
            {
                Text = App.GetString("/Controls/DeleteItem"),
                Tag = message.User.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.Delete)
            };
            //delete.Click += FlyoutManager.;

            // If current user
            if (LocalState.CurrentUser.Id == message.User.Id)
            {
                // Add "Edit" button
                menu.Items.Add(edit);
            }

            // Null check Guild (for DMs)
            if (App.CurrentGuildId != null)
            {
                // If permissions to delete message
                if (LocalState.Guilds[App.CurrentGuildId].channels[message.ChannelId].permissions.ManageMessages || LocalState.Guilds[App.CurrentGuildId].channels[message.ChannelId].permissions.Administrator || message?.User.Id == LocalState.CurrentUser.Id || LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId == LocalState.CurrentUser.Id)
                {
                    // Add "Delete" button
                    menu.Items.Add(delete);
                }
            }

            // If dev mode
            if (Storage.Settings.DevMode)
            {

                // Add Separator
                MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
                menu.Items.Add(sep1);

                // Add "CopyId" button
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
