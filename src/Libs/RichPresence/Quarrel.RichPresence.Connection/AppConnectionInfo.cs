// Quarrel © 2022

namespace Quarrel.RichPresence
{
    public static class AppConnectionInfo
    {
        public const string QuarrelPackageFamilyName = "38062AvishaiDernis.DiscordUWP_q72k3wbnqqnj6";
        public const string QuarrelInsiderPackageFamilyName = "38062AvishaiDernis.DiscordUWPInsider_q72k3wbnqqnj6";
        public const string QuarrelAlphaPackageFamilyName = "38062AvishaiDernis.DiscordUWPAlpha_6x7nhkcjyfy2y";
        public const string QuarrelDevPackageFamilyName = "38062AvishaiDernis.DiscordUWPDev_q72k3wbnqqnj6";

        public const string RichPresenceServiceName = "Quarrel.RichPresenceAPI";

        public static string GetPackageFamilyName(AppVersionType versionType)
        {
            return versionType switch
            {
                AppVersionType.Dev => QuarrelDevPackageFamilyName,
                AppVersionType.Alpha => QuarrelAlphaPackageFamilyName,
                AppVersionType.Insider => QuarrelInsiderPackageFamilyName,
                _ or AppVersionType.Release => QuarrelPackageFamilyName,
            };
        }
    }
}
