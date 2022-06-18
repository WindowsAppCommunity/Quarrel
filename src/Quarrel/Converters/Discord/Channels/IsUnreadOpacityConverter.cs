// Quarrel © 2022

namespace Quarrel.Converters.Discord.Channels
{
    public class IsUnreadOpacityConverter
    {
        public static double Convert(bool isUnread) => isUnread ? 1 : 0.7;
    }
}
