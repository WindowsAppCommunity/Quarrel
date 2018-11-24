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

using Discord_UWP.Managers;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeSavePictureFlyout(string url)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

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
