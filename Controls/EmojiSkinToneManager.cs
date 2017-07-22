using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Controls
{
    static class EmojiSkinToneManager
    {
        public static string ChangeSkinTone(this string emoji,
        int skinTone)
        {
            emoji = emoji.Replace("🏻", "").Replace("🏼", "").Replace("🏽", "").Replace("🏾", "").Replace("🏿", "");
            if (skinTone == 0) return emoji;
            else
                emoji += GetSkinTone(skinTone);
            return emoji;
        }
        private static string GetSkinTone(int skintone)
        {
            switch (skintone)
            {
                case 1: return "🏻";
                case 2: return "🏼";
                case 3: return "🏽";
                case 4: return "🏾";
                case 5: return "🏿";
            }
            return "";
        }
    }
}
