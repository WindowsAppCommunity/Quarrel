using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Ryken.Devices;

namespace Quarrel.StateTriggers
{
    /// <summary>
    /// Trigger to differentiate between device families
    /// </summary>
    public class DeviceFamilyStateTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty TargetDeviceFamilyProperty = DependencyProperty.Register(
            "TargetDeviceFamily", typeof(DeviceFamily), typeof(DeviceFamilyStateTrigger), new PropertyMetadata(default(DeviceFamily), OnDeviceTypePropertyChanged));

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
    public enum DeviceFamily
    {
        Unidentified,
        Desktop,
        Mobile,
        Xbox,
        Holographic,
        IoT,
        Team,
    }

    /// <summary>
    /// Retrieves strongly-typed device family
    /// </summary>
    public static class DeviceFamilyHelper
    {
        static DeviceFamilyHelper()
        {
            DeviceFamily = RecognizeDeviceFamily(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
        }

        public static DeviceFamily DeviceFamily { get; }

        private static DeviceFamily RecognizeDeviceFamily(string deviceFamily)
        {
            switch (deviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFamily.Mobile;
                case "Windows.Desktop":
                    return DeviceFamily.Desktop;
                case "Windows.Xbox":
                    return DeviceFamily.Xbox;
                case "Windows.Holographic":
                    return DeviceFamily.Holographic;
                case "Windows.IoT":
                    return DeviceFamily.IoT;
                case "Windows.Team":
                    return DeviceFamily.Team;
                default:
                    return DeviceFamily.Unidentified;
            }
        }
    }
}
