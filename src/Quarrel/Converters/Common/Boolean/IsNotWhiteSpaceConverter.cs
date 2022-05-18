// Quarrel © 2022

namespace Quarrel.Converters.Common.Boolean
{
    public class IsNotWhiteSpaceConverter
    {
        public static bool Convert(string? item1) => !string.IsNullOrWhiteSpace(item1);
    }
}
