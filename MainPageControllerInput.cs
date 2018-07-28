using Discord_UWP.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Discord_UWP.SubPages;

namespace Discord_UWP
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

            if (MessageList.Items.Count > 0)
                MessageList.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            else if (MessageArea.Visibility == Windows.UI.Xaml.Visibility.Visible)
                MessageBox1.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            else if (friendPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
                friendPanel.Focus(Windows.UI.Xaml.FocusState.Keyboard);

        }

        private void SideDrawer_DrawOpenedRight(object sender, EventArgs e)
        {
            if(MembersListView.Items.Count>0)
                MembersListView.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private void SideDrawer_DrawOpenedLeft(object sender, EventArgs e)
        {
            if (ChannelList.SelectedItem != null)
                ChannelList.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            else
                ServerList.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Shift)
            {
                MessageBox1.ShiftDown();
            }
            else if (args.VirtualKey == VirtualKey.S)
            {
                if (args.KeyStatus.IsKeyReleased && CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control) != CoreVirtualKeyStates.None)
                {
                    SubFrameNavigator(typeof(SubPages.DiscordStatus));
                }
                else
                {
                    args.Handled = true;
                }
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadLeftThumbstickLeft)
            {
                // args.Handled = true;
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadLeftThumbstickRight)
            {
                //  args.Handled = true;
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadLeftThumbstickUp)
            {
                // args.Handled = true;
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadLeftThumbstickDown)
            {
                //  args.Handled = true;
                //  ScrollviewerFromGamepad();
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadLeftShoulder)
            {
                if (SubFrame.Visibility == Windows.UI.Xaml.Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.RepeatCount == 1 && !args.KeyStatus.IsKeyReleased)
                    sideDrawer.ToggleLeft();
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadRightShoulder)
            {
                if (SubFrame.Visibility == Windows.UI.Xaml.Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.RepeatCount == 1 && !args.KeyStatus.IsKeyReleased)
                    sideDrawer.ToggleRight();
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadView)
            {
                if (SubFrame.Visibility == Windows.UI.Xaml.Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.IsKeyReleased)
                {
                    MenuHint.Release();
                    MenuHint.ContextFlyout.ShowAt(MenuHint);
                }
                else
                    MenuHint.Press();
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadX)
            {
                if (SubFrame.Visibility == Windows.UI.Xaml.Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.IsKeyReleased)
                {
                    XHint.Release();
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
                else
                {
                    XHint.Press();
                }
            }
            /*
            else if (args.VirtualKey == Windows.System.VirtualKey.GamepadY)
            {
                if (SubFrame.Visibility == Windows.UI.Xaml.Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.IsKeyReleased)
                {
                    YHint.Release();
                    var focused = (Windows.UI.Xaml.DependencyObject)FocusManager.GetFocusedElement();
                    
                    if (focused.GetType() == typeof(ListViewItem))
                    {
                        var type = ((ListViewItem)focused).Content.GetType();
                        Point position = new Point(0, 0);

                        if (type == typeof(SimpleGuild))
                        {
                            var guild = ((SimpleGuild)((ListViewItem)focused).Content);
                            App.ShowMenuFlyout(focused, Managers.FlyoutManager.Type.Guild, guild.Id, null, position);
                        }
                        else if (type == typeof(SimpleChannel))
                        {
                            var channel = ((SimpleChannel)((ListViewItem)focused).Content);
                            Managers.FlyoutManager.Type flyouttype = Managers.FlyoutManager.Type.TextChn;
                            if (channel.Type == 1)
                                flyouttype = Managers.FlyoutManager.Type.DMChn;
                            else if (channel.Type == 3)
                                flyouttype = Managers.FlyoutManager.Type.GroupChn;
                            else return;
                            App.ShowMenuFlyout(focused, flyouttype, channel.Id, null, position);
                        }
                        else if (type == typeof(KeyValuePair<string, Member>))
                        {
                            var member = ((KeyValuePair<string, Member>)((ListViewItem)focused).Content).Value;
                            App.ShowMenuFlyout(focused, Managers.FlyoutManager.Type.GuildMember, member.Raw.User.Id, App.CurrentGuildId, position);
                        }
                    }
                }
                else
                {
                    YHint.Press();
                }


                // args.Handled = true;
            }*/
        }
        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
 
        }
        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Shift)
            {
                MessageBox1.ShiftUp();
            }
            if (args.VirtualKey == Windows.System.VirtualKey.GamepadMenu)
            {

            }

            else if(args.VirtualKey == Windows.System.VirtualKey.GamepadY)
            {
                YHint.Release();
            }
        }


        private void KeyboardAccelerator_OnInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            SubFrameNavigator(typeof(DiscordStatus));
        }
    }
}