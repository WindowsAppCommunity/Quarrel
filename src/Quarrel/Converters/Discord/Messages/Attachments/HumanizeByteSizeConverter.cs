// Quarrel © 2022

using Humanizer;
using Humanizer.Bytes;

namespace Quarrel.Converters.Discord.Messages.Attachments
{
    public class HumanizeByteSizeConverter
    {
        public static string Convert(ulong bytes)
        {
            var byteSize = new ByteSize(bytes);
            return byteSize.Humanize("0.00");
        }
    }
}
