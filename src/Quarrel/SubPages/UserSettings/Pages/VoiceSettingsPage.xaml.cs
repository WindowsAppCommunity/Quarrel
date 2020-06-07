// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using System;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// The user settings Voice page.
    /// </summary>
    public sealed partial class VoiceSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceSettingsPage"/> class.
        /// </summary>
        public VoiceSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new VoiceSettingsViewModel();
            LoadDevices();
        }

        /// <summary>
        /// Gets the app's voice settings.
        /// </summary>
        public VoiceSettingsViewModel ViewModel => this.DataContext as VoiceSettingsViewModel;

        private async void LoadDevices()
        {
            DeviceInformationCollection outputs = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender).AsTask();
            foreach (var device in outputs)
            {
                OutputDevices.Items.Add(device);
                if (device.IsEnabled && ViewModel.OutputDeviceId == device.Id)
                {
                    OutputDevices.SelectedItem = device;
                }
            }

            if (OutputDevices.SelectedItem == null)
            {
                OutputDevices.SelectedIndex = 0;
            }

            DeviceInformationCollection inputs = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture).AsTask();
            foreach (var device in inputs)
            {
                InputDevices.Items.Add(device);
                if (device.IsEnabled && ViewModel.InputDeviceId == device.Id)
                {
                    InputDevices.SelectedItem = device;
                }
            }

            if (InputDevices.SelectedItem == null)
            {
                InputDevices.SelectedIndex = 0;
            }
        }

        private void OutputDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.OutputDeviceId = (OutputDevices.SelectedItem as DeviceInformation).Id;
        }

        private void InputDeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.InputDeviceId = (InputDevices.SelectedItem as DeviceInformation).Id;
        }
    }
}
