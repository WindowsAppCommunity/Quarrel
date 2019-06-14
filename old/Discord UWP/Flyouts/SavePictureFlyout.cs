using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using Quarrel.Managers;

namespace Quarrel.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeSavePictureFlyout(string url)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add "Save Picture" button
            MenuFlyoutItem Save = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/SavePicture"),
                Tag = url,
                Icon = new SymbolIcon(Symbol.Save)
            };
            Save.Click += FlyoutManager.SavePicture;
            menu.Items.Add(Save);

            return menu;
        }
    }
}
