// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// Voice settings page data.
    /// </summary>
    public class VoiceSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the default output device is the selected output device.
        /// </summary>
        public bool DefaultOutput
        {
            get => SettingsService.Roaming.GetValue<AudioDevice>(SettingKeys.OutputDevice) == AudioDevice.Default;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                SettingsService.Roaming.SetValue(SettingKeys.OutputDevice, AudioDevice.Default);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the communications device output device is the selected output device.
        /// </summary>
        public bool CommunicationsOutput
        {
            get => SettingsService.Roaming.GetValue<AudioDevice>(SettingKeys.OutputDevice) == AudioDevice.Communiciations;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                SettingsService.Roaming.SetValue(SettingKeys.OutputDevice, AudioDevice.Communiciations);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the default output device is the selected output device.
        /// </summary>
        public bool DefaultInput
        {
            get => SettingsService.Roaming.GetValue<AudioDevice>(SettingKeys.InputDevice) == AudioDevice.Default;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                SettingsService.Roaming.SetValue(SettingKeys.InputDevice, AudioDevice.Default);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the communications device output device is the selected output device.
        /// </summary>
        public bool CommunicationsInput
        {
            get => SettingsService.Roaming.GetValue<AudioDevice>(SettingKeys.InputDevice) == AudioDevice.Communiciations;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                SettingsService.Roaming.SetValue(SettingKeys.InputDevice, AudioDevice.Communiciations);
            }
        }

        private ISettingsService SettingsService { get; } = SimpleIoc.Default.GetInstance<ISettingsService>();
    }
}
