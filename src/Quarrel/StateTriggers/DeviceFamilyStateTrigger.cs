// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Helpers;
using Windows.UI.Xaml;

namespace Quarrel.StateTriggers
{
    /// <summary>
    /// Trigger to differentiate between device families.
    /// </summary>
    public class DeviceFamilyStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// A property representing the app's current device type target.
        /// </summary>
        public static readonly DependencyProperty TargetDeviceFamilyProperty = DependencyProperty.Register(
            "TargetDeviceFamily", typeof(DeviceFamily), typeof(DeviceFamilyStateTrigger), new PropertyMetadata(default(DeviceFamily), OnDeviceTypePropertyChanged));

        /// <summary>
        /// Gets or sets the device type the app is targeting.
        /// </summary>
        public DeviceFamily TargetDeviceFamily
        {
            get => (DeviceFamily)GetValue(TargetDeviceFamilyProperty);
            set => SetValue(TargetDeviceFamilyProperty, value);
        }

        private static void OnDeviceTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var trigger = (DeviceFamilyStateTrigger)dependencyObject;
            var newTargetDeviceFamily = (DeviceFamily)eventArgs.NewValue;
            trigger.SetActive(newTargetDeviceFamily == DeviceFamilyHelper.DeviceFamily);
        }
    }
}
