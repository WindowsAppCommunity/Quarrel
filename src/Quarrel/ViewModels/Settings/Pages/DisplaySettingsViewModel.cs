using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Settings;
using Quarrel.Services.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Settings.Pages
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
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.Bluple);
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, true, notify : true);
            }
        }

        public bool SystemAccentColor
        {
            get => !SettingsService.Roaming.GetValue<bool>(SettingKeys.Bluple);
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, false, notify : true);
            }
        }

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
    }
}
