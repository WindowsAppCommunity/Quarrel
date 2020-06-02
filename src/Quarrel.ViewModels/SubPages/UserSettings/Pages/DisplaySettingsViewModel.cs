// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// Display settings page data.
    /// </summary>
    public class DisplaySettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the Theme setting is set to Dark.
        /// </summary>
        public bool Dark
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Dark;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Dark, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Theme setting is set to Light.
        /// </summary>
        public bool Light
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Light;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Light, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Theme setting is set to Windows.
        /// </summary>
        public bool Windows
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Windows;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Windows, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Theme setting is set to Discord.
        /// </summary>
        public bool Discord
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.Discord;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.Discord, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Theme setting is set to OLED.
        /// </summary>
        public bool OLED
        {
            get => SettingsService.Roaming.GetValue<Theme>(SettingKeys.Theme) == Theme.OLED;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Theme, Theme.OLED, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the blurple setting is set to <see langword="true"/>.
        /// </summary>
        public bool Blurple
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.Blurple);
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Blurple, true, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the blurple setting is set to <see langword="false"/>.
        /// </summary>
        public bool SystemAccentColor
        {
            get => !SettingsService.Roaming.GetValue<bool>(SettingKeys.Blurple);
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.Blurple, false, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the fluent theme setting is set to <see langword="true"/>.
        /// </summary>
        public bool FluentTheme
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.FluentTheme);
            set => SettingsService.Roaming.SetValue(SettingKeys.FluentTheme, value, true, true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message view acrylic is enabled.
        /// </summary>
        public bool MessageViewAcrylic
        {
            get => (AcrylicSettings & AcrylicSettings.MessageView) == AcrylicSettings.MessageView;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, AcrylicSettings ^ AcrylicSettings.MessageView, true, true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the channel view acrylic is enabled.
        /// </summary>
        public bool ChannelViewAcrylic
        {
            get => (AcrylicSettings & AcrylicSettings.ChannelView) == AcrylicSettings.ChannelView;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, AcrylicSettings ^ AcrylicSettings.ChannelView, true, true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the guild view acrylic is enabled.
        /// </summary>
        public bool GuildViewAcrylic
        {
            get => (AcrylicSettings & AcrylicSettings.GuildView) == AcrylicSettings.GuildView;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, AcrylicSettings ^ AcrylicSettings.GuildView, true, true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the commandbar view acrylic is enabled.
        /// </summary>
        public bool CommandBarAcrylic
        {
            get => (AcrylicSettings & AcrylicSettings.CommandBar) == AcrylicSettings.CommandBar;
            set => SettingsService.Roaming.SetValue(SettingKeys.AcrylicSettings, AcrylicSettings ^ AcrylicSettings.CommandBar, true, true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the server mute icons setting is set to <see langword="true"/>.
        /// </summary>
        public bool ServerMuteIcons
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.ServerMuteIcons);
            set => SettingsService.Roaming.SetValue(SettingKeys.ServerMuteIcons, value, notify: true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the expensive rendering setting is set to <see langword="true"/>.
        /// </summary>
        public bool ExpensiveRendering
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering);
            set => SettingsService.Roaming.SetValue(SettingKeys.ExpensiveRendering, value, notify: true);
        }

        /// <summary>
        /// Gets or sets the font size settings.
        /// </summary>
        public int FontSize
        {
            get => SettingsService.Roaming.GetValue<int>(SettingKeys.FontSize);
            set => SettingsService.Roaming.SetValue(SettingKeys.FontSize, value, notify: true);
        }

        private ISettingsService SettingsService { get; } = SimpleIoc.Default.GetInstance<ISettingsService>();

        private AcrylicSettings AcrylicSettings => SettingsService.Roaming.GetValue<AcrylicSettings>(SettingKeys.AcrylicSettings);
    }
}
