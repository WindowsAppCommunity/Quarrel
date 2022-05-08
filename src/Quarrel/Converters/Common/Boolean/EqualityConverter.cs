// Quarrel © 2022

namespace Quarrel.Converters.Common.Boolean
{
    public sealed class EqualityConverter
    {
        public static bool Convert(object item1, object item2) => item1.Equals(item2);
    }
}
