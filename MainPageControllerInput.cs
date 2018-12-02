using Discord_UWP.SimpleClasses;
using System;
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
            if (args.VirtualKey == VirtualKey.Shift)
            {
           //     MessageBox1.ShiftDown();
            }
            /*
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
            }*/
            else if (args.VirtualKey == VirtualKey.GamepadLeftThumbstickLeft)
            {
                // args.Handled = true;
            }
            else if (args.VirtualKey == VirtualKey.GamepadLeftThumbstickRight)
            {
                //  args.Handled = true;
            }
            else if (args.VirtualKey == VirtualKey.GamepadLeftThumbstickUp)
            {
                // args.Handled = true;
            }
            else if (args.VirtualKey == VirtualKey.GamepadLeftThumbstickDown)
            {
                //  args.Handled = true;
                //  ScrollviewerFromGamepad();
            }
            else if (args.VirtualKey == VirtualKey.GamepadLeftShoulder)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.RepeatCount == 1 && !args.KeyStatus.IsKeyReleased)
                    sideDrawer.ToggleLeft();

                if (args.KeyStatus.IsKeyReleased)
                {
                    LBumperHint.Release();
                }
                else
                {
                    LBumperHint.Press();
                }
            }
            else if (args.VirtualKey == VirtualKey.GamepadRightShoulder)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.RepeatCount == 1 && !args.KeyStatus.IsKeyReleased)
                    sideDrawer.ToggleRight();

                if (args.KeyStatus.IsKeyReleased)
                {
                    RBumperHint.Release();
                }
                else
                {
                    RBumperHint.Press();
                }
            }
            else if (args.VirtualKey == VirtualKey.GamepadView)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                args.Handled = true;
                if (args.KeyStatus.IsKeyReleased)
                {
                    MenuHint.Release();
                    MenuHint.ContextFlyout.ShowAt(MenuHint);
                }
                else
                {
                    MenuHint.Press();
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