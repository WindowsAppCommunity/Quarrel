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
    public class BehaviorSettingsViewModel : ViewModelBase
    {
        private ISettingsService SettingsService = SimpleIoc.Default.GetInstance<ISettingsService>();

        public bool MentionGlow
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.MentionGlow);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.MentionGlow, value, notify : true);
            }
        }

        public bool ShowNoPermissions
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.ShowNoPermssions);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.ShowNoPermssions, value, notify : true);
            }
        }

        #region Collapse Override

        public bool NoCollapseOverride
        {
            get => SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride) == CollapseOverride.None;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.None, notify : true);
            }
        }

        public bool MentionCollapseOverride
        {
            get => SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride) == CollapseOverride.Mention;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.Mention, notify : true);
            }
        }

        public bool UnreadCollapseOverride
        {
            get => SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride) == CollapseOverride.Unread;
            set
            {
                if (value)
                    SettingsService.Roaming.SetValue(SettingKeys.CollapseOverride, CollapseOverride.Unread, notify : true);
            }
        }

        #endregion

        public bool TTLAttachments
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.TTLAttachments);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.TTLAttachments, value, notify : true);
            }
        }

        public bool DataCompression
        {
            get => SettingsService.Roaming.GetValue<bool>(SettingKeys.DataCompression);
            set
            {
                SettingsService.Roaming.SetValue(SettingKeys.DataCompression, value, notify : true);
            }
        }
    }
}
