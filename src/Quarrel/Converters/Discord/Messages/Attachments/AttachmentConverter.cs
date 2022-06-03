// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Converters.Discord.Messages.Attachments
{
    internal class AttachmentConverter
    {
        public static int ConvertWidth(int width, int height, int maxWidth, int maxHeight)
        {
            if (width > maxWidth || height > maxHeight)
            {
                if (width > height)
                {
                    return maxWidth;
                }
                else
                {
                    return (int)Math.Round((double)width / height * maxHeight);
                }
            }
            else
            {
                return width;
            }
        }

        public static int ConvertHeight(int width, int height, int maxWidth = 400, int maxHeight = 300)
        {
            if (width > maxWidth || height > maxHeight)
            {
                if (width > height)
                {
                    return (int)Math.Round((double)height / width * maxWidth);
                }

                return maxHeight;
            }
            else
            {
                return height;
            }
        }

        public static string ConvertImageUrl(string url, int width, int height, int maxWidth, int maxHeight)
        {
            int realWidth = ConvertWidth(width, height, maxWidth, maxHeight);
            int realHeight = ConvertHeight(width, height, maxWidth, maxHeight);
            return $"{url}?width={realWidth}&height={realHeight}";
        }
    }
}
