// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Settings;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// Voice settings page data.
    /// </summary>
    public class VoiceSettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the Id of the output device to use for audio.
        /// </summary>
        public string OutputDeviceId
        {
            get => SettingsService.Roaming.GetValue<string>(SettingKeys.OutputDevice);
            set => SettingsService.Roaming.SetValue<string>(SettingKeys.OutputDevice, value, true, true);
        }

        /// <summary>
        /// Gets or sets the Id of the output device to use for audio.
        /// </summary>
        public string InputDeviceId
        {
            get => SettingsService.Roaming.GetValue<string>(SettingKeys.InputDevice);
            set => SettingsService.Roaming.SetValue<string>(SettingKeys.InputDevice, value, true, true);
        }

        private ISettingsService SettingsService { get; } = SimpleIoc.Default.GetInstance<ISettingsService>();
    }
}
