using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class DisplaySettingsViewModel : ViewModelBase
    {
        private ISettingsService SettingsService = SimpleIoc.Default.GetInstance<ISettingsService>();

        #region Theme

        public bool Dark
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Dark;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Dark, notify: true);
            }
        }

        public bool Light
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Light;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Light, notify: true);
            }
        }

        public bool Windows
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Windows;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Windows, notify: true);
            }
        }

        public bool Discord
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Discord;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Discord, notify: true);
            }
        }

        public bool OLED
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.OLED;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.OLED, notify: true);
            }
        }

        #endregion

        #region Accent Color

        public bool Blurple
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.Blurple);
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Blurple, true, notify : true);
            }
        }

        public bool SystemAccentColor
        {
            get => !SettingsService.Roaming.GetValue<bool>(SettingKeys.Blurple);
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Blurple, false, notify : true);
            }
        }

        #endregion

        #region Acrylic

        public bool MessageViewAcrylic
        {
            get => (_AcrylicSettings & AcrylicSettings.MessageView) == AcrylicSettings.MessageView;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, _AcrylicSettings ^ AcrylicSettings.MessageView, true, true);
        }

        public bool ChannelViewAcrylic
        {
            get => (_AcrylicSettings & AcrylicSettings.ChannelView) == AcrylicSettings.ChannelView;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, _AcrylicSettings ^ AcrylicSettings.ChannelView, true, true);
        }

        public bool GuildViewAcrylic
        {
            get => (_AcrylicSettings & AcrylicSettings.GuildView) == AcrylicSettings.GuildView;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, _AcrylicSettings ^ AcrylicSettings.GuildView, true, true);
        }

        public bool CommandBarAcrylic
        {
            get => (_AcrylicSettings & AcrylicSettings.CommandBar) == AcrylicSettings.CommandBar;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, _AcrylicSettings ^ AcrylicSettings.CommandBar, true, true);
        }

        private AcrylicSettings _AcrylicSettings => SettingsService.Roaming.GetValue<AcrylicSettings>(SettingKeys.AcrylicSettings);

        #endregion

        public bool ServerMuteIcons
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.ServerMuteIcons);
            set => SettingsService.Roaming.SetValue(SettingKeys.ServerMuteIcons, value, notify: true);
        }

        public bool ExpensiveRendering
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering);
            set => SettingsService.Roaming.SetValue(SettingKeys.ExpensiveRendering, value, notify: true);
        }
        public int FontSize
        {
            get => SettingsService.Roaming.GetValue<int>(SettingKeys.FontSize);
            set => SettingsService.Roaming.SetValue(SettingKeys.FontSize, value, notify: true);
        }
    }
}
