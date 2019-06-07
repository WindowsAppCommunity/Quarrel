using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Classes
{
    /// <summary>
    /// Used for fancy-text nicknames
    /// </summary>
    public class FancyText
    {
        /// <summary>
        /// Initalize converts for Fancy Generator
        /// </summary>
        /// <param name="baseType">Type the current text is</param>
        public FancyText(Converters baseType)
        {
            // Add a conversion from the current type to each other type
            foreach (Converters type in ConverterValues)
            {
                FancyConverters.Add(type, new FancyConverter(baseType, type));
            }
        }

        /// <summary>
        /// Class that converts from one text format to another
        /// </summary>
        public class FancyConverter
        {
            /// <summary>
            /// Setup converter
            /// </summary>
            /// <param name="from">Input text-format</param>
            /// <param name="to">Output text-format</param>
            public FancyConverter(Converters from, Converters to)
            {
                CharacterConversions = new Dictionary<string, string>();
                ReplaceDiacritics = true;

                // The "Reversed" requires Pre/Post processing
                if (to == Converters.Reversed)
                {
                    PostProcess = input => ReverseString(input.ToLower());
                }
                if (from == Converters.Reversed)
                {
                    PreProcess = input => ReverseString(Common.CapitalizeMulti(input));
                }

                // Key-Value the character in the text formats
                var fromList = GetListFromConverter(from);
                var toList = GetListFromConverter(to);
                for (int i = 0; i < fromList.Count-1; i++)
                {
                    if (CharacterConversions.ContainsKey(fromList[i]))
                    {
                        CharacterConversions.Remove(fromList[i]); //Default to capitals
                    }
                    CharacterConversions.Add(fromList[i], toList[i]);
                }
            }

            public Dictionary<string, string> CharacterConversions { get; set; }
            public Func<string, string> PreProcess { get; set; }
            public Func<string, string> PostProcess { get; set; }
            public bool IgnoreMissingChars { get; set; }
            public bool ReplaceDiacritics { get; set; }
            public bool Random { get; set; }
        }

        /// <summary>
        /// Get Character list from converter enum
        /// </summary>
        /// <param name="converter">The list to get</param>
        /// <returns>Character list of the <paramref name="converter"/> type</returns>
        public static List<string> GetListFromConverter(Converters converter)
        {
            switch (converter)
            {
                case Converters.Standard:
                    return Standard;
                case Converters.Circled:
                    return Circled;
                case Converters.Script:
                    return Script;
                case Converters.ScriptBold:
                    return ScriptBold;
                case Converters.Gothic:
                    return Gothic;
                case Converters.GothicBold:
                    return GothicBold;
                case Converters.Hollow:
                    return Hollow;
                case Converters.Money:
                    return Money;
                case Converters.TheGreatTuna:
                    return TheGreatTuna;
                case Converters.Reversed:
                    return Reversed;
                case Converters.Typewriter:
                    return Typewriter;
                case Converters.Spacious:
                    return Spacious;
            }
            return Standard;
        }
        
        /// <summary>
        /// Reverse string <paramref name="s"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns><paramref name="s"/> with the characters in reverse order</returns>
        public static string ReverseString(string s)
        {
            if (s == null) return null;
            char[] charArray = s.ToCharArray();
            int len = s.Length - 1;
            
            for (int i = 0; i < len; i++, len--)
            {
                // Copy the values at i to len and vice versa
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }

            return new string(charArray);
        }
        public enum Converters { Standard, Circled, Script, ScriptBold, Gothic, GothicBold, Hollow, Money, TheGreatTuna, Reversed, Typewriter, Random, Spacious }

        #region Raw Converter CharLists
        public static List<string> Standard = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static List<string> Circled = new List<string> { "0", "\u2460", "\u2461", "\u2462", "\u2463", "\u2464", "\u2465", "\u2466", "\u2467", "\u2468", "\u24d0", "\u24d1", "\u24d2", "\u24d3", "\u24d4", "\u24d5", "\u24d6", "\u24d7", "\u24d8", "\u24d9", "\u24da", "\u24db", "\u24dc", "\u24dd", "\u24de", "\u24df", "\u24e0", "\u24e1", "\u24e2", "\u24e3", "\u24e4", "\u24e5", "\u24e6", "\u24e7", "\u24e8", "\u24e9", "\u24b6", "\u24b7", "\u24b8", "\u24b9", "\u24ba", "\u24bb", "\u24bc", "\u24bd", "\u24be", "\u24bf", "\u24c0", "\u24c1", "\u24c2", "\u24c3", "\u24c4", "\u24c5", "\u24c6", "\u24c7", "\u24c8", "\u24c9", "\u24ca", "\u24cb", "\u24cc", "\u24cd", "\u24ce", "\u24cf" };
        public static List<string> Script = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "𝒶", "𝒷", "𝒸", "𝒹", "𝑒", "𝒻", "𝑔", "𝒽", "𝒾", "𝒿", "𝓀", "𝓁", "𝓂", "𝓃", "𝑜", "𝓅", "𝓆", "𝓇", "𝓈", "𝓉", "𝓊", "𝓋", "𝓌", "𝓍", "𝓎", "𝓏", "𝓐", "𝓑", "𝓒", "𝓓", "𝓔", "𝓕", "𝓖", "𝓗", "𝓘", "𝓙", "𝓚", "𝓛", "𝓜", "𝓝", "𝓞", "𝓟", "𝓠", "𝓡", "𝓢", "𝓣", "𝓤", "𝓥", "𝓦", "𝓧", "𝓨", "𝓩" };
        public static List<string> ScriptBold = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "𝓪", "𝓫", "𝓬", "𝓭", "𝓮", "𝓯", "𝓰", "𝓱", "𝓲", "𝓳", "𝓴", "𝓵", "𝓶", "𝓷", "𝓸", "𝓹", "𝓺", "𝓻", "𝓼", "𝓽", "𝓾", "𝓿", "𝔀", "𝔁", "𝔂", "𝔃", "𝓐", "𝓑", "𝓒", "𝓓", "𝓔", "𝓕", "𝓖", "𝓗", "𝓘", "𝓙", "𝓚", "𝓛", "𝓜", "𝓝", "𝓞", "𝓟", "𝓠", "𝓡", "𝓢", "𝓣", "𝓤", "𝓥", "𝓦", "𝓧", "𝓨", "𝓩" };
        public static List<string> Gothic = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "𝔞", "𝔟", "𝔠", "𝔡", "𝔢", "𝔣", "𝔤", "𝔥", "𝔦", "𝔧", "𝔨", "𝔩", "𝔪", "𝔫", "𝔬", "𝔭", "𝔮", "𝔯", "𝔰", "𝔱", "𝔲", "𝔳", "𝔴", "𝔵", "𝔶", "𝔷", "𝔄", "𝔅", "ℭ", "𝔇", "𝔈", "𝔉", "𝔊", "ℌ", "ℑ", "𝔍", "𝔎", "𝔏", "𝔐", "𝔑", "𝔒", "𝔓", "𝔔", "ℜ", "𝔖", "𝔗", "𝔘", "𝔙", "𝔚", "𝔛", "𝔜", "ℨ" };
        public static List<string> GothicBold = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "𝖆", "𝖇", "𝖈", "𝖉", "𝖊", "𝖋", "𝖌", "𝖍", "𝖎", "𝖏", "𝖐", "𝖑", "𝖒", "𝖓", "𝖔", "𝖕", "𝖖", "𝖗", "𝖘", "𝖙", "𝖚", "𝖛", "𝖜", "𝖝", "𝖞", "𝖟", "𝕬", "𝕭", "𝕮", "𝕯", "𝕰", "𝕱", "𝕲", "𝕳", "𝕴", "𝕵", "𝕶", "𝕷", "𝕸", "𝕹", "𝕺", "𝕻", "𝕼", "𝕽", "𝕾", "𝕿", "𝖀", "𝖁", "𝖂", "𝖃", "𝖄", "𝖅" };
        public static List<string> Hollow = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "𝕒", "𝕓", "𝕔", "𝕕", "𝕖", "𝕗","𝕘", "𝕙", "𝕚", "𝕛", "𝕜", "𝕝", "𝕞", "𝕟", "𝕠", "𝕡", "𝕢", "𝕣", "𝕤", "𝕥", "𝕦", "𝕧", "𝕨", "𝕩", "𝕪", "𝕫", "𝔸", "𝔹", "ℂ", "𝔻", "𝔼", "𝔽", "𝔾", "ℍ", "𝕀", "𝕁", "𝕂", "𝕃", "𝕄", "ℕ", "𝕆", "ℙ", "ℚ", "ℝ", "𝕊", "𝕋", "𝕌", "𝕍", "𝕎", "𝕏", "𝕐", "ℤ" };
        public static List<string> Money = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "₳", "฿", "₵", "Đ", "Ɇ", "₣", "₲", "Ⱨ", "ł", "J", "₭", "Ⱡ", "₥", "₦", "Ø", "₱", "Q", "Ɽ", "₴", "₮", "Ʉ", "V", "₩", "Ӿ", "Ɏ", "₴", "₳", "฿", "₵", "Đ", "Ɇ", "₣", "₲", "Ⱨ", "ł", "J", "₭", "Ⱡ", "₥", "₦", "Ø", "₱", "Q", "Ɽ", "₴", "₮", "Ʉ", "V", "₩", "Ӿ", "Ɏ", "₴" };
        public static List<string> TheGreatTuna = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Λ", "B", "₵", "Đ", "Ξ", "Ϝ", "G", "H", "I", "J", "K", "L", "Ϻ", "Π", "ϴ", "Ρ", "Ϙ", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Λ", "B", "₵", "Đ", "Ξ", "Ϝ", "G", "H", "I", "J", "K", "L", "Ϻ", "Π", "ϴ", "Ρ", "Ϙ", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static List<string> Reversed = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "ɐ", "q", "ɔ","p", "ǝ","ɟ", "ɓ", "ɥ", "ı", "ɾ", "ʞ", "l", "ɯ", "u", "o", "p", "q", "ɹ", "s", "ʇ", "u", "ʌ", "ʍ", "x", "ʎ", "z", "ɐ", "q", "ɔ", "p", "ǝ", "ɟ", "ɓ", "ɥ", "ı", "ɾ", "ʞ", "l", "ɯ", "u", "o", "p", "q", "ɹ", "s", "ʇ", "u", "ʌ", "ʍ", "x", "ʎ", "z" };
        public static List<string> Typewriter = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "𝚊", "𝚋", "𝚌", "𝚍", "𝚎", "𝚏", "𝚐", "𝚑", "𝚒", "𝚓", "𝚔", "𝚕", "𝚖", "𝚗", "𝚘", "𝚙", "𝚚", "𝚛", "𝚜", "𝚝", "𝚞", "𝚟", "𝚠", "𝚡", "𝚢", "𝚣", "𝙰", "𝙱", "𝙲", "𝙳", "𝙴", "𝙵", "𝙶", "𝙷", "𝙸", "𝙹", "𝙺", "𝙻", "𝙼", "𝙽", "𝙾", "𝙿", "𝚀", "𝚁", "𝚂", "𝚃", "𝚄", "𝚅", "𝚆", "𝚇", "𝚈", "𝚉"};
        public static List<string> Spacious = new List<string> { "０", "１", "２", "３", "４", "５", "６", "７", "８", "９", "ａ", "ｂ", "ｃ", "ｄ", "ｅ", "ｆ", "ｇ", "ｈ", "ｉ", "ｊ", "ｋ", "ｌ", "ｍ", "ｎ", "ｏ", "ｐ", "ｑ", "ｒ", "ｓ", "ｔ", "ｕ", "ｖ", "ｗ", "ｘ", "ｙ", "ｚ", "Ａ", "Ｂ", "Ｃ", "Ｄ", "Ｅ", "Ｆ", "Ｇ", "Ｈ", "Ｉ", "Ｊ", "Ｋ", "Ｌ", "Ｍ", "Ｎ", "Ｏ", "Ｐ", "Ｑ", "Ｒ", "Ｓ", "Ｔ", "Ｕ", "Ｖ", "Ｗ", "Ｘ", "Ｙ", "Ｚ"};
        #endregion

        public Converters[] ConverterValues = new Converters[]
        {
            Converters.Standard,
            Converters.Circled,
            Converters.Script,
            Converters.ScriptBold,
            Converters.Gothic,
            Converters.GothicBold,
            Converters.Hollow,
            Converters.Money,
            Converters.TheGreatTuna,
            Converters.Reversed,
            Converters.Typewriter,
            Converters.Spacious
            //Converters.Random
        };
        
        /// <summary>
        /// Dictionary of Converts by Converter enum
        /// </summary>
        public Dictionary<Converters, FancyConverter> FancyConverters = new Dictionary<Converters, FancyConverter>();


        /// <summary>
        /// Convert the input using all available converters
        /// </summary>
        /// <param name="input">The text to convert</param>
        /// <returns></returns>
        public List<string> ConvertAll(string input)
        {
            Stopwatch sw = new Stopwatch();
            List<string> results = new List<string>();
            int counter = 0;
            foreach (Converters converter in ConverterValues)
            {
                results.Add(MakeFancy(converter, input));
                counter++;
            }
            sw.Stop();
            //Debug.WriteLine(counter + " FancyTextGenerators converted " + input.Length + " characters in " + sw.ElapsedMilliseconds + "ms");
            return results;
        }

        // Random used for getting a random converter
        Random rnd = new Random();

        /// <summary>
        /// Convert the text with the specified converter
        /// TODO: Improve "random" feature, don't know what I was thinking
        /// </summary>
        /// <param name="converter">The text converter</param>
        /// <param name="input">The text to convert</param>
        /// <returns>The transformed text</returns>
        public string MakeFancy(Converters converter, string input)
        {
            // The get converter by Converter enum
            FancyConverter convert = FancyConverters[converter];

            if (convert.ReplaceDiacritics)
                input = Common.RemoveDiacritics(input);
            if (convert.PreProcess != null)
                input = convert.PreProcess(input);
            string output = "";
            
            // Convert each character and it to output when done
            for (int i = 0; i < input.Length;)
            {
                // If the converter is random, just pick a random different converter and convert as that (if not also random
                if (convert.Random)
                {
                    FancyConverter randomconverter = FancyConverters[ConverterValues[rnd.Next(0, ConverterValues.Length-1)]];
                    if (!randomconverter.Random)
                    {
                        string character = input[i].ToString();
                        if (Char.IsSurrogate(input[i]))
                        {
                            character = new string(new[] { input[i], input[++i] });
                        }
                        if (randomconverter.CharacterConversions.ContainsKey(character))
                            output += randomconverter.CharacterConversions[character];
                        else if (!randomconverter.IgnoreMissingChars)
                            output += character;
                        i++;
                    }
                }
                else
                {
                    string character = input[i].ToString();
                    if (Char.IsSurrogate(input[i]))
                    {
                        character = new string(new[] { input[i], input[++i] });
                    }
                    if (convert.CharacterConversions.ContainsKey(character))
                        output += convert.CharacterConversions[character];
                    else if (!convert.IgnoreMissingChars)
                        output += character;
                    i++;
                }
            }

            // Run the post process
            if (convert.PostProcess != null)
                output = convert.PostProcess(output);

            return output;  
        }

        /// <summary>
        /// Determine the text-format of <paramref name="input"/>
        /// </summary>
        /// <param name="input">string to check</param>
        /// <returns>text-format of <paramref name="input"/> as Converter enum</returns>
        public static Converters FindFancy(string input)
        {
            string c = input[0].ToString();
            int i = 0;
            while (i < input.Length && Common.IsEnLetter(input[i]) && !Char.IsSurrogate(input[i]))
            {
                i++;
            }

            if (i >= input.Length)
            {
                return Converters.Standard;
            }

            if (Char.IsSurrogate(input[i]))
            {
                c = new string(new[] { input[i], input[++i] });
            } else
            {
                c = input[i].ToString();
            }

            if (Standard.Contains(c))
            {
                return Converters.Standard;
            }
            else if (Circled.Contains(c))
            {
                return Converters.Circled;
            }
            else if (Script.Contains(c))
            {
                return Converters.Script;
            }
            else if (ScriptBold.Contains(c))
            {
                return Converters.ScriptBold;
            }
            else if (Gothic.Contains(c))
            {
                return Converters.Gothic;
            }
            else if (GothicBold.Contains(c))
            {
                return Converters.GothicBold;
            }
            else if (Hollow.Contains(c))
            {
                return Converters.Hollow;
            }
            else if (Money.Contains(c))
            {
                return Converters.Money;
            }
            else if (TheGreatTuna.Contains(c))
            {
                return Converters.TheGreatTuna;
            }
            else if (Reversed.Contains(c))
            {
                return Converters.Reversed;
            }
            else if (Typewriter.Contains(c))
            {
                return Converters.Typewriter;
            }
            else if (Spacious.Contains(c))
            {
                return Converters.Spacious;
            } else
            {
                return Converters.Standard;
            }
        }
    }
}
