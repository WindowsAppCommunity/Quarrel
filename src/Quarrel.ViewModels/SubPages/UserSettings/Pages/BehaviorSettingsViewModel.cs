// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// Behavior settings page data.
    /// </summary>
    public class BehaviorSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the mention glow settings is true.
        /// </summary>
        public bool MentionGlow
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.MentionGlow);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.MentionGlow, value, true, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to show the presence glow around a message's author icon.
        /// </summary>
        public bool ShowAuthorPresence
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.AuthorPresence);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.AuthorPresence, value, true, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to show no permissions channels settings is true.
        /// </summary>
        public bool ShowNoPermissions
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.ShowNoPermssions);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.ShowNoPermssions, value, true, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the collapse override is set to none.
        /// </summary>
        public bool NoCollapseOverride
        {
            get => SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride) == CollapseOverride.None;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.None, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the collapse override is set to on mentions.
        /// </summary>
        public bool MentionCollapseOverride
        {
            get => SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride) == CollapseOverride.Mention;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.Mention, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the collapse override is set to unread.
        /// </summary>
        public bool UnreadCollapseOverride
        {
            get => SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride) == CollapseOverride.Unread;
            set
            {
                if (value)
                {
                    SettingsService.Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.Unread, true, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the filter members settings is set to true.
        /// </summary>
        public bool FilterMembers
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.FilterMembers);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.FilterMembers, value, true, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tap to load settings is set to true.
        /// </summary>
        public bool TTLAttachments
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.TTLAttachments);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.TTLAttachments, value, true, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the data compression settings is set to true.
        /// </summary>
        public bool DataCompression
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.DataCompression);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.DataCompression, value, true, true);
            }
        }

        private ISettingsService SettingsService { get; } = SimpleIoc.Default.GetInstance<ISettingsService>();
    }
}
