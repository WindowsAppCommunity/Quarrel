// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Helpers
{
    /// <summary>
    /// Target device type.
    /// </summary>
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
    /// Retrieves strongly-typed device family.
    /// </summary>
    public static class DeviceFamilyHelper
    {
        static DeviceFamilyHelper()
        {
            DeviceFamily = RecognizeDeviceFamily(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
        }

        /// <summary>
        /// Gets the current device's device family.
        /// </summary>
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
