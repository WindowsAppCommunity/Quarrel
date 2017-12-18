using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Discord_UWP
{
    public sealed partial class MainPage : Page
    {
        private async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if(args.VirtualKey == Windows.System.VirtualKey.GamepadLeftShoulder)
            {
                sideDrawer.ToggleLeft();
            }
            if(args.VirtualKey == Windows.System.VirtualKey.GamepadRightTrigger)
            {
                sideDrawer.ToggleRight();
            }
        }
    }
}