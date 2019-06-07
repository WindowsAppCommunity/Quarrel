using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Quarrel.SimpleClasses;
using Quarrel.SubPages;

namespace Quarrel
{
    public sealed partial class MainPage : Page
    {
        private void ScrollviewerFromGamepad()
        {
           // var reading = Gamepad.Gamepads.First().GetCurrentReading();
           // MessageScrollviewer.ChangeView(null, MessageScrollviewer.VerticalOffset - (reading.RightThumbstickY * 100), null);
        }

        private void SideDrawer_DrawsClosed(object sender, EventArgs e)
        {
/*
            if (MessageList.Items.Count > 0)
                MessageList.Focus(FocusState.Keyboard);
            else if (MessageArea.Visibility == Visibility.Visible)
                MessageBox1.Focus(FocusState.Keyboard);
            else if (friendPanel.Visibility == Visibility.Visible)
                friendPanel.Focus(FocusState.Keyboard);*/

        }

        private void SideDrawer_DrawOpenedRight(object sender, EventArgs e)
        {
            if(MembersListView.Items.Count>0)
                MembersListView.Focus(FocusState.Keyboard);
        }

        private void SideDrawer_DrawOpenedLeft(object sender, EventArgs e)
        {
            if (ChannelList.SelectedItem != null)
                ChannelList.Focus(FocusState.Keyboard);
            else
                ServerList.Focus(FocusState.Keyboard);
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            App.HandleKeyPress(args.VirtualKey, args.KeyStatus.IsKeyReleased);
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.GamepadMenu)
            {
                SelectHint.Press();
            }
            else if (args.VirtualKey == VirtualKey.GamepadX)
            {
                //XHint.Press();
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Shift)
            {
                //    MessageBox1.ShiftUp();
            }
            else if (args.VirtualKey == VirtualKey.GamepadMenu)
            {
                SelectHint.Release();
            }
            else if (args.VirtualKey == VirtualKey.GamepadX)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                args.Handled = true;
                //XHint.Release();
                var focused = (Windows.UI.Xaml.DependencyObject)FocusManager.GetFocusedElement();
                if (focused.GetType() == typeof(ListViewItem))
                {
                    var type = ((ListViewItem)focused).Content.GetType();
                    if (type == typeof(SimpleChannel) || type == typeof(SimpleGuild))
                    {
                        userButton.Flyout.ShowAt(userButton);
                    }
                }
            }
        }

        private void KeyboardAccelerator_OnInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            SubFrameNavigator(typeof(DiscordStatus));
        }
    }
}