using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Discord_UWP
{
    public sealed partial class MainPage : Page
    {
        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            
            if(args.VirtualKey == Windows.System.VirtualKey.GamepadLeftShoulder)
            {
                sideDrawer.ToggleLeft();
            }
            else if(args.VirtualKey == Windows.System.VirtualKey.GamepadLeftShoulder)
            {
                sideDrawer.ToggleRight();
            }
            else if(args.VirtualKey == Windows.System.VirtualKey.GamepadMenu)
            {
                MenuHint.Press();
            }
            else if(args.VirtualKey == Windows.System.VirtualKey.GamepadView)
            {
                ViewHint.Press();
            }
            else if(args.VirtualKey == Windows.System.VirtualKey.GamepadY)
            {
                YHint.Press();
                if(((Control)FocusManager.GetFocusedElement()).Parent == ServerList)
                {
                    App.ShowMenuFlyout(ServerList.SelectedItem, Managers.FlyoutManager.Type.Guild, ((Managers.GuildManager.SimpleGuild)ServerList.SelectedItem).Id, null, new Windows.Foundation.Point(0, 0));
                }
            }
        }
        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.GamepadMenu)
            {
                MenuHint.Release();
                MenuHint.ContextFlyout.ShowAt(YHint);
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadView)
            {
                ViewHint.Release();
            }
            else if(args.VirtualKey == Windows.System.VirtualKey.GamepadY)
            {
                YHint.Release();
                
            }
        }
    }
}