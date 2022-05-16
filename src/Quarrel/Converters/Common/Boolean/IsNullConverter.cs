// Quarrel © 2022

namespace Quarrel.Converters.Common.Boolean
{
    public class IsNullConverter
    {
        public static bool Convert(object? item1) => item1 is null;
    }
}
