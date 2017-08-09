using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            header.Text = App.Translate("Settings");
            UIItem.Header = App.Translate("UI");
            ResponsiveUIText.Text = App.Translate("ResponsiveUI").ToUpper();
            AppBarAtBottom_checkbox.Content = App.Translate("AllignAppbarToBottom");
            ExpensiveUI.Content = App.Translate("ExpensiveRender");
            ThemeText.Text = App.Translate("Theme").ToUpper();
            RestartRequiredText.Text = App.Translate("RestartRequired");
            radio_Dark.Content = App.Translate("Dark");
            radio_Light.Content = App.Translate("Light");
            AccentColorText.Text = App.Translate("AccentColor");
            CompactMode.Content = App.Translate("CompactMode");
            NotificationsItem.Header = App.Translate("Notifications");
            HighlightEveryone.Content = App.Translate("Highlight") + " @everyone";
            Vibrate.Content = App.Translate("VibrateOnMessage");
            Toasts.Content = App.Translate("ShowToastsInApp");
            AdvancedItem.Header = App.Translate("Advanced");
            ResponsiveUIBreakpointText.Text = App.Translate("ResponsiveUIBreakpoint").ToUpper();
            ResponsiveUIBreakpointDesc.Text = App.Translate("ResponsiveUIBreakpointDesc");
            RespUI_M.Header = "M(" + App.Translate("Servers") + ")";
            RespUI_L.Header = "L(" + App.Translate("Servers") + " + " + App.Translate("Channels") + ")";
            RespUI_L.Header = "L(" + App.Translate("Servers") + " + " + App.Translate("Channels") + ")";
            RespUI_L.Header = "XL(" + App.Translate("Servers") + " + " + App.Translate("Channels") + " + "  + App.Translate("Members") + ")";
            ResetDefaultBreakpoints.Content = App.Translate("ResetDefaultBreakpoints");
            OnlineColor.Text = App.Translate("Online") + " " + App
                .Translate("Color");
            IdleColor.Text = App.Translate("Idle") + " " + App.Translate("Color");
            DndColor.Text = App.Translate("Dnd") + " " + App.Translate("Color");

            logout.Content = App.Translate("Logout");
            button.Content = App.Translate("Cancel");
            SaveButton.Content = App.Translate("Save");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            HighlightEveryone.IsChecked = Storage.Settings.HighlightEveryone;
            Toasts.IsChecked = Storage.Settings.Toasts;
            Vibrate.IsChecked = Storage.Settings.Vibrate;
            CompactMode.IsChecked = Storage.Settings.CompactMode;
            RespUI_M.Value = Storage.Settings.RespUiM;
            RespUI_L.Value = Storage.Settings.RespUiL;
            RespUI_XL.Value = Storage.Settings.RespUiXl;
            AppBarAtBottom_checkbox.IsChecked = Storage.Settings.AppBarAtBottom;
            accent_combobox.SelectedItem = accent_combobox.Items.FirstOrDefault(x => (((ComboBoxItem)x).Tag as SolidColorBrush).Color.ToHex() == Storage.Settings.AccentBrush);
            ExpensiveUI.IsChecked = Storage.Settings.ExpensiveRender;

            if (Storage.Settings.Theme == Theme.Dark)
                radio_Dark.IsChecked = true;
            else if (Storage.Settings.Theme == Theme.Light)
                radio_Light.IsChecked = true;
            else if (Storage.Settings.Theme == Theme.Windows)
                radio_Windows.IsChecked = true;
            else if (Storage.Settings.Theme == Theme.Discord)
                radio_Discord.IsChecked = true;
        }

        private void rootgrid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void SaveUserSettings(object sender, RoutedEventArgs e)
        {
            Storage.Settings.HighlightEveryone = (bool)HighlightEveryone.IsChecked;
            Storage.Settings.Toasts = (bool)Toasts.IsChecked;
            Storage.Settings.Vibrate = (bool)Vibrate.IsChecked;

            Storage.Settings.RespUiM = RespUI_M.Value;
            Storage.Settings.RespUiL = RespUI_L.Value;
            Storage.Settings.RespUiXl = RespUI_XL.Value;
            Storage.Settings.AppBarAtBottom = (bool)AppBarAtBottom_checkbox.IsChecked;
            Storage.Settings.CompactMode = (bool)CompactMode.IsChecked;
            Storage.Settings.AccentBrush = ((SolidColorBrush)(accent_combobox.SelectedItem as ComboBoxItem)?.Tag)?.Color.ToHex();
            Storage.Settings.OnlineBursh = ((SolidColorBrush)(online_colorbox.SelectedItem as ComboBoxItem)?.Tag)?.Color.ToHex();
            Storage.Settings.IdleBrush = ((SolidColorBrush)(idle_colorbox.SelectedItem as ComboBoxItem)?.Tag)?.Color.ToHex();
            Storage.Settings.DndBrush = ((SolidColorBrush)(dnd_colorbox.SelectedItem as ComboBoxItem)?.Tag)?.Color.ToHex();
            Storage.Settings.ExpensiveRender = (bool)ExpensiveUI.IsChecked;

            if ((bool)radio_Dark.IsChecked)
                Storage.Settings.Theme = Theme.Dark;
            else if ((bool)radio_Light.IsChecked)
                Storage.Settings.Theme = Theme.Light;
            else if ((bool)radio_Windows.IsChecked)
                Storage.Settings.Theme = Theme.Windows;
            else if ((bool)radio_Discord.IsChecked)
                Storage.Settings.Theme = Theme.Discord;

            Storage.SaveAppSettings();
            Storage.SettingsChanged();
            CloseButton_Click(null, null);
        }

        bool _ignoreRespUiChanges = false;
        private void RespUI_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_ignoreRespUiChanges)
            {
                if (RespUI_L.Value < RespUI_M.Value) RespUI_L.Value = RespUI_M.Value;
                if (RespUI_XL.Value < RespUI_L.Value) RespUI_XL.Value = RespUI_L.Value;
            }
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            _ignoreRespUiChanges = true;
            RespUI_M.Value = 569;
            RespUI_L.Value = 768;
            RespUI_XL.Value = 1024;
            _ignoreRespUiChanges = false;
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private async void CheckLogout(object sender, RoutedEventArgs e)
        {
            MessageDialog winnerAnounce = new MessageDialog(App.GetString("/Settings/VerifyLogout"));
            winnerAnounce.Commands.Add(new UICommand(App.GetString("/Settings/LogoutBTN.Content"),
        new UICommandInvokedHandler(ConfirmLogout)));
            winnerAnounce.Commands.Add(new UICommand(App.GetString("/Dialogs/CancelBTN.Content"),
                new UICommandInvokedHandler(CancelLogout)));
            await winnerAnounce.ShowAsync();
        }
        private void CancelLogout(IUICommand command)
        {

        }
        private void ConfirmLogout(IUICommand command)
        {
            Storage.Clear();
            Session.Logout();
            (Window.Current.Content as Frame).Navigate(typeof(LockScreen), null);
        }
    }
}
