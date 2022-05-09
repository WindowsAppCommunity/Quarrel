// Quarrel © 2022

namespace Quarrel.Converters.Discord.Messages
{
    public class IsDeletedToOpacityConverter
    {
        public static double Convert(bool isDeleted)
        {
            if (isDeleted) return 0.5;
            return 1;
        }
    }
}
