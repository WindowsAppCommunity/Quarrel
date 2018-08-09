using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Classes
{
    public class FancyText
    {
        public class FancyGenerator
        {
            public Dictionary<char, string> CharacterConversions { get; set; }
            public Func<string, string> PreProcess { get; set; }
            public Func<string, string> PostProcess { get; set; }
            public bool IgnoreMissingChars { get; set; }
            public bool ReplaceDiacritics { get; set; }
            public bool Random { get; set; }
        }
        public static string ReverseString(string s)
        {
            if (s == null) return null;
            char[] charArray = s.ToCharArray();
            int len = s.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }

            return new string(charArray);
        }
        public enum Converters { Circled, Script, ScriptBold, Gothic, GothicBold, Hollow, Money, TheGreatTuna, Reversed, Typewriter, Random, Spacious }

        public Converters[] ConverterValues = new Converters[]
        {
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
            Converters.Random,
            Converters.Spacious
        };
        public Dictionary<Converters, FancyGenerator> FancyConverters =
            new Dictionary<Converters, FancyGenerator>()
            {
                {Converters.Circled, new FancyGenerator()
                    {
                        ReplaceDiacritics = true,
                        CharacterConversions = new Dictionary<char, string>()
                        {{'1', "\u2460"},
                            {'2', "\u2461"},
                            {'3', "\u2462"},
                            {'4', "\u2463"},
                            {'5', "\u2464"},
                            {'6', "\u2465"},
                            {'7', "\u2466"},
                            {'8', "\u2467"},
                            {'9', "\u2468"},
                            {'a', "\u24d0"},
                            {'b', "\u24d1"},
                            {'c', "\u24d2"},
                            {'d', "\u24d3"},
                            {'e', "\u24d4"},
                            {'f', "\u24d5"},
                            {'g', "\u24d6"},
                            {'h', "\u24d7"},
                            {'i', "\u24d8"},
                            {'j', "\u24d9"},
                            {'k', "\u24da"},
                            {'l', "\u24db"},
                            {'m', "\u24dc"},
                            {'n', "\u24dd"},
                            {'o', "\u24de"},
                            {'p', "\u24df"},
                            {'q', "\u24e0"},
                            {'r', "\u24e1"},
                            {'s', "\u24e2"},
                            {'t', "\u24e3"},
                            {'u', "\u24e4"},
                            {'v', "\u24e5"},
                            {'w', "\u24e6"},
                            {'x', "\u24e7"},
                            {'y', "\u24e8"},
                            {'z', "\u24e9"},
                            {'A', "\u24b6"},
                            {'B', "\u24b7"},
                            {'C', "\u24b8"},
                            {'D', "\u24b9"},
                            {'E', "\u24ba"},
                            {'F', "\u24bb"},
                            {'G', "\u24bc"},
                            {'H', "\u24bd"},
                            {'I', "\u24be"},
                            {'J', "\u24bf"},
                            {'K', "\u24c0"},
                            {'L', "\u24c1"},
                            {'M', "\u24c2"},
                            {'N', "\u24c3"},
                            {'O', "\u24c4"},
                            {'P', "\u24c5"},
                            {'Q', "\u24c6"},
                            {'R', "\u24c7"},
                            {'S', "\u24c8"},
                            {'T', "\u24c9"},
                            {'U', "\u24ca"},
                            {'V', "\u24cb"},
                            {'W', "\u24cc"},
                            {'X', "\u24cd"},
                            {'Y', "\u24ce"},
                            {'Z', "\u24cf"}
                        }
                    }},
                {Converters.Script, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    CharacterConversions = new Dictionary<char, string>()
                    {
                        {'a', "𝒶" },
                        {'b', "𝒷" },
                        {'c', "𝒸" },
                        {'d', "𝒹" },
                        {'e', "𝑒" },
                        {'f', "𝒻" },
                        {'g', "𝑔" },
                        {'h', "𝒽" },
                        {'i', "𝒾" },
                        {'j', "𝒿" },
                        {'k', "𝓀" },
                        {'l', "𝓁" },
                        {'m', "𝓂" },
                        {'n', "𝓃" },
                        {'o', "𝑜" },
                        {'p', "𝓅" },
                        {'q', "𝓆" },
                        {'r', "𝓇" },
                        {'s', "𝓈" },
                        {'t', "𝓉" },
                        {'u', "𝓊" },
                        {'v', "𝓋" },
                        {'w', "𝓌" },
                        {'x', "𝓍" },
                        {'y', "𝓎" },
                        {'z', "𝓏" },
                        {'A', "𝓐" },
                        {'B', "𝓑" },
                        {'C', "𝓒" },
                        {'D', "𝓓" },
                        {'E', "𝓔" },
                        {'F', "𝓕" },
                        {'G', "𝓖" },
                        {'H', "𝓗" },
                        {'I', "𝓘" },
                        {'J', "𝓙" },
                        {'K', "𝓚" },
                        {'L', "𝓛" },
                        {'M', "𝓜" },
                        {'N', "𝓝" },
                        {'O', "𝓞" },
                        {'P', "𝓟" },
                        {'Q', "𝓠" },
                        {'R', "𝓡" },
                        {'S', "𝓢" },
                        {'T', "𝓣" },
                        {'U', "𝓤" },
                        {'V', "𝓥" },
                        {'W', "𝓦" },
                        {'X', "𝓧" },
                        {'Y', "𝓨" },
                        {'Z', "𝓩" }
                    }
                }},
                {Converters.ScriptBold, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    CharacterConversions = new Dictionary<char, string>()
                    {
                        {'a', "𝓪"},
                        {'b', "𝓫"},
                        {'c', "𝓬"},
                        {'d', "𝓭"},
                        {'e', "𝓮"},
                        {'f', "𝓯"},
                        {'g', "𝓰"},
                        {'h', "𝓱"},
                        {'i', "𝓲"},
                        {'j', "𝓳"},
                        {'k', "𝓴"},
                        {'l', "𝓵"},
                        {'m', "𝓶"},
                        {'n', "𝓷"},
                        {'o', "𝓸"},
                        {'p', "𝓹"},
                        {'q', "𝓺"},
                        {'r', "𝓻"},
                        {'s', "𝓼"},
                        {'t', "𝓽"},
                        {'u', "𝓾"},
                        {'v', "𝓿"},
                        {'w', "𝔀"},
                        {'x', "𝔁"},
                        {'y', "𝔂"},
                        {'z', "𝔃"},
                        {'A', "𝓐"},
                        {'B', "𝓑"},
                        {'C', "𝓒"},
                        {'D', "𝓓"},
                        {'E', "𝓔"},
                        {'F', "𝓕"},
                        {'G', "𝓖"},
                        {'H', "𝓗"},
                        {'I', "𝓘"},
                        {'J', "𝓙"},
                        {'K', "𝓚"},
                        {'L', "𝓛"},
                        {'M', "𝓜"},
                        {'N', "𝓝"},
                        {'O', "𝓞"},
                        {'P', "𝓟"},
                        {'Q', "𝓠"},
                        {'R', "𝓡"},
                        {'S', "𝓢"},
                        {'T', "𝓣"},
                        {'U', "𝓤"},
                        {'V', "𝓥"},
                        {'W', "𝓦"},
                        {'X', "𝓧"},
                        {'Y', "𝓨"},
                        {'Z', "𝓩"},
                    }
                }},
                {Converters.Gothic, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    CharacterConversions = new Dictionary<char, string>()
                    {
                        {'a', "𝔞"},
                        {'b', "𝔟"},
                        {'c', "𝔠"},
                        {'d', "𝔡"},
                        {'e', "𝔢"},
                        {'f', "𝔣"},
                        {'g', "𝔤"},
                        {'h', "𝔥"},
                        {'i', "𝔦"},
                        {'j', "𝔧"},
                        {'k', "𝔨"},
                        {'l', "𝔩"},
                        {'m', "𝔪"},
                        {'n', "𝔫"},
                        {'o', "𝔬"},
                        {'p', "𝔭"},
                        {'q', "𝔮"},
                        {'r', "𝔯"},
                        {'s', "𝔰"},
                        {'t', "𝔱"},
                        {'u', "𝔲"},
                        {'v', "𝔳"},
                        {'w', "𝔴"},
                        {'x', "𝔵"},
                        {'y', "𝔶"},
                        {'z', "𝔷"},
                        {'A', "𝔄"},
                        {'B', "𝔅"},
                        {'C', "ℭ"},
                        {'D', "𝔇"},
                        {'E', "𝔈"},
                        {'F', "𝔉"},
                        {'G', "𝔊"},
                        {'H', "ℌ"},
                        {'I', "ℑ"},
                        {'J', "𝔍"},
                        {'K', "𝔎"},
                        {'L', "𝔏"},
                        {'M', "𝔐"},
                        {'N', "𝔑"},
                        {'O', "𝔒"},
                        {'P', "𝔓"},
                        {'Q', "𝔔"},
                        {'R', "ℜ"},
                        {'S', "𝔖"},
                        {'T', "𝔗"},
                        {'U', "𝔘"},
                        {'V', "𝔙"},
                        {'W', "𝔚"},
                        {'X', "𝔛"},
                        {'Y', "𝔜"},
                        {'Z', "ℨ"}
                    }
                }},
                {Converters.GothicBold, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    CharacterConversions = new Dictionary<char, string>()
                    {
                        {'a', "𝖆"},
                        {'b', "𝖇"},
                        {'c', "𝖈"},
                        {'d', "𝖉"},
                        {'e', "𝖊"},
                        {'f', "𝖋"},
                        {'g', "𝖌"},
                        {'h', "𝖍"},
                        {'i', "𝖎"},
                        {'j', "𝖏"},
                        {'k', "𝖐"},
                        {'l', "𝖑"},
                        {'m', "𝖒"},
                        {'n', "𝖓"},
                        {'o', "𝖔"},
                        {'p', "𝖕"},
                        {'q', "𝖖"},
                        {'r', "𝖗"},
                        {'s', "𝖘"},
                        {'t', "𝖙"},
                        {'u', "𝖚"},
                        {'v', "𝖛"},
                        {'w', "𝖜"},
                        {'x', "𝖝"},
                        {'y', "𝖞"},
                        {'z', "𝖟"},
                        {'A', "𝕬"},
                        {'B', "𝕭"},
                        {'C', "𝕮"},
                        {'D', "𝕯"},
                        {'E', "𝕰"},
                        {'F', "𝕱"},
                        {'G', "𝕲"},
                        {'H', "𝕳"},
                        {'I', "𝕴"},
                        {'J', "𝕵"},
                        {'K', "𝕶"},
                        {'L', "𝕷"},
                        {'M', "𝕸"},
                        {'N', "𝕹"},
                        {'O', "𝕺"},
                        {'P', "𝕻"},
                        {'Q', "𝕼"},
                        {'R', "𝕽"},
                        {'S', "𝕾"},
                        {'T', "𝕿"},
                        {'U', "𝖀"},
                        {'V', "𝖁"},
                        {'W', "𝖂"},
                        {'X', "𝖃"},
                        {'Y', "𝖄"},
                        {'Z', "𝖅"}
                        }
                        }},
                {Converters.Hollow, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    CharacterConversions = new Dictionary<char, string>()
                    {{'a', "𝕒"},
                        {'b', "𝕓"},
                        {'c', "𝕔"},
                        {'d', "𝕕"},
                        {'e', "𝕖"},
                        {'f', "𝕗"},
                        {'g', "𝕘"},
                        {'h', "𝕙"},
                        {'i', "𝕚"},
                        {'j', "𝕛"},
                        {'k', "𝕜"},
                        {'l', "𝕝"},
                        {'m', "𝕞"},
                        {'n', "𝕟"},
                        {'o', "𝕠"},
                        {'p', "𝕡"},
                        {'q', "𝕢"},
                        {'r', "𝕣"},
                        {'s', "𝕤"},
                        {'t', "𝕥"},
                        {'u', "𝕦"},
                        {'v', "𝕧"},
                        {'w', "𝕨"},
                        {'x', "𝕩"},
                        {'y', "𝕪"},
                        {'z', "𝕫"},
                        {'A', "𝔸"},
                        {'B', "𝔹"},
                        {'C', "ℂ"},
                        {'D', "𝔻"},
                        {'E', "𝔼"},
                        {'F', "𝔽"},
                        {'G', "𝔾"},
                        {'H', "ℍ"},
                        {'I', "𝕀"},
                        {'J', "𝕁"},
                        {'K', "𝕂"},
                        {'L', "𝕃"},
                        {'M', "𝕄"},
                        {'N', "ℕ"},
                        {'O', "𝕆"},
                        {'P', "ℙ"},
                        {'Q', "ℚ"},
                        {'R', "ℝ"},
                        {'S', "𝕊"},
                        {'T', "𝕋"},
                        {'U', "𝕌"},
                        {'V', "𝕍"},
                        {'W', "𝕎"},
                        {'X', "𝕏"},
                        {'Y', "𝕐"},
                        {'Z', "ℤ"}
                        }
                }},
                {Converters.Money, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    PreProcess = input => input.ToUpper(),
                    CharacterConversions= new Dictionary<char, string>()
                    {
                        {'A', "₳"},
                        {'B', "฿"},
                        {'C', "₵"},
                        {'D', "Đ"},
                        {'E', "Ɇ"},
                        {'F', "₣"},
                        {'G', "₲"},
                        {'H', "Ⱨ"},
                        {'I', "ł"},
                        {'J', "J"},
                        {'K', "₭"},
                        {'L', "Ⱡ"},
                        {'M', "₥"},
                        {'N', "₦"},
                        {'O', "Ø"},
                        {'P', "₱"},
                        {'Q', "Q"},
                        {'R', "Ɽ"},
                        {'S', "₴"},
                        {'T', "₮"},
                        {'U', "Ʉ"},
                        {'V', "V"},
                        {'W', "₩"},
                        {'X', "Ӿ"},
                        {'Y', "Ɏ"},
                        {'Z', "₴"}
                    }
                }},
                {Converters.TheGreatTuna, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    PreProcess = input => input.ToUpper(),
                    CharacterConversions= new Dictionary<char, string>()
                    {
                        {'A', "Λ"},
                        {'B', "B"},
                        {'C', "₵"},
                        {'D', "Đ"},
                        {'E', "Ξ"},
                        {'F', "Ϝ"},
                        {'G', "G"},
                        {'H', "H"},
                        {'I', "I"},
                        {'J', "J"},
                        {'K', "K"},
                        {'L', "L"},
                        {'M', "Ϻ"},
                        {'N', "Π"},
                        {'O', "ϴ"},
                        {'P', "Ρ"},
                        {'Q', "Ϙ"},
                        {'R', "R"},
                        {'S', "S"},
                        {'T', "T"},
                        {'U', "U"},
                        {'V', "V"},
                        {'W', "W"},
                        {'X', "X"},
                        {'Y', "Y"},
                        {'Z', "Z"}
                    }
                }},
                {Converters.Reversed, new FancyGenerator()
                {
                    ReplaceDiacritics = true,
                    PreProcess = input => ReverseString(input.ToLower()),
                    CharacterConversions= new Dictionary<char, string>()
                    {
                        {'a', "ɐ"},
                        {'b', "q"},
                        {'c', "ɔ"},
                        {'d', "p"},
                        {'e', "ǝ"},
                        {'f', "ɟ"},
                        {'g', "ɓ"},
                        {'h', "ɥ"},
                        {'i', "ı"},
                        {'j', "ɾ"},
                        {'k', "ʞ"},
                        {'l', "l"},
                        {'m', "ɯ"},
                        {'n', "u"},
                        {'o', "o"},
                        {'p', "p"},
                        {'q', "q"},
                        {'r', "ɹ"},
                        {'s', "s"},
                        {'t', "ʇ"},
                        {'u', "u"},
                        {'v', "ʌ"},
                        {'w', "ʍ"},
                        {'x', "x"},
                        {'y', "ʎ"},
                        {'z', "z"}
                    }
                }},
                {Converters.Typewriter, new FancyGenerator()
                {
                    ReplaceDiacritics=true,
                    CharacterConversions=new Dictionary<char, string>()
                    {
                        {'a', "𝚊"},
                        {'b', "𝚋"},
                        {'c', "𝚌"},
                        {'d', "𝚍"},
                        {'e', "𝚎"},
                        {'f', "𝚏"},
                        {'g', "𝚐"},
                        {'h', "𝚑"},
                        {'i', "𝚒"},
                        {'j', "𝚓"},
                        {'k', "𝚔"},
                        {'l', "𝚕"},
                        {'m', "𝚖"},
                        {'n', "𝚗"},
                        {'o', "𝚘"},
                        {'p', "𝚙"},
                        {'q', "𝚚"},
                        {'r', "𝚛"},
                        {'s', "𝚜"},
                        {'t', "𝚝"},
                        {'u', "𝚞"},
                        {'v', "𝚟"},
                        {'w', "𝚠"},
                        {'x', "𝚡"},
                        {'y', "𝚢"},
                        {'z', "𝚣"},
                        {'A', "𝙰"},
                        {'B', "𝙱"},
                        {'C', "𝙲"},
                        {'D', "𝙳"},
                        {'E', "𝙴"},
                        {'F', "𝙵"},
                        {'G', "𝙶"},
                        {'H', "𝙷"},
                        {'I', "𝙸"},
                        {'J', "𝙹"},
                        {'K', "𝙺"},
                        {'L', "𝙻"},
                        {'M', "𝙼"},
                        {'N', "𝙽"},
                        {'O', "𝙾"},
                        {'P', "𝙿"},
                        {'Q', "𝚀"},
                        {'R', "𝚁"},
                        {'S', "𝚂"},
                        {'T', "𝚃"},
                        {'U', "𝚄"},
                        {'V', "𝚅"},
                        {'W', "𝚆"},
                        {'X', "𝚇"},
                        {'Y', "𝚈"},
                        {'Z', "𝚉"}
                    }
                }},
                {Converters.Spacious, new FancyGenerator()
                {
                    ReplaceDiacritics=true,
                    CharacterConversions=new Dictionary<char, string>()
                    {

                        {'a', "ａ"},
                        {'b', "ｂ"},
                        {'c', "ｃ"},
                        {'d', "ｄ"},
                        {'e', "ｅ"},
                        {'f', "ｆ"},
                        {'g', "ｇ"},
                        {'h', "ｈ"},
                        {'i', "ｉ"},
                        {'j', "ｊ"},
                        {'k', "ｋ"},
                        {'l', "ｌ"},
                        {'m', "ｍ"},
                        {'n', "ｎ"},
                        {'o', "ｏ"},
                        {'p', "ｐ"},
                        {'q', "ｑ"},
                        {'r', "ｒ"},
                        {'s', "ｓ"},
                        {'t', "ｔ"},
                        {'u', "ｕ"},
                        {'v', "ｖ"},
                        {'w', "ｗ"},
                        {'x', "ｘ"},
                        {'y', "ｙ"},
                        {'z', "ｚ"},
                        {'A', "Ａ"},
                        {'B', "Ｂ"},
                        {'C', "Ｃ"},
                        {'D', "Ｄ"},
                        {'E', "Ｅ"},
                        {'F', "Ｆ"},
                        {'G', "Ｇ"},
                        {'H', "Ｈ"},
                        {'I', "Ｉ"},
                        {'J', "Ｊ"},
                        {'K', "Ｋ"},
                        {'L', "Ｌ"},
                        {'M', "Ｍ"},
                        {'N', "Ｎ"},
                        {'O', "Ｏ"},
                        {'P', "Ｐ"},
                        {'Q', "Ｑ"},
                        {'R', "Ｒ"},
                        {'S', "Ｓ"},
                        {'T', "Ｔ"},
                        {'U', "Ｕ"},
                        {'V', "Ｖ"},
                        {'W', "Ｗ"},
                        {'X', "Ｘ"},
                        {'Y', "Ｙ"},
                        {'Z', "Ｚ"},
                        {'0', "０"},
                        {'1', "１"},
                        {'2', "２"},
                        {'3', "３"},
                        {'4', "４"},
                        {'5', "５"},
                        {'6', "６"},
                        {'7', "７"},
                        {'8', "８"},
                        {'9', "９"}

                    }
                }},
                {Converters.Random, new FancyGenerator()
                {
                    Random = true
                } }
            };
                    

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
            foreach(Converters converter in ConverterValues)
            {
                results.Add(MakeFancy(converter, input));
                counter++;
            }
            sw.Stop();
            Debug.WriteLine(counter + " FancyTextGenerators converted " + input.Length + " characters in " + sw.ElapsedMilliseconds + "ms");
            return results;
        }

        Random rnd = new Random();
        /// <summary>
        /// Convert the text with the specified converter
        /// </summary>
        /// <param name="converter">The text converter</param>
        /// <param name="input">The text to convert</param>
        /// <returns>The transformed text</returns>
        public string MakeFancy(Converters converter, string input)
        {
            var convert = FancyConverters[converter];

            if (convert.ReplaceDiacritics)
                input = Common.RemoveDiacritics(input);
            if (convert.PreProcess != null)
                input = convert.PreProcess(input);
            string output = "";
            for (var i = 0; i < input.Length;)
            {
                if (convert.Random)
                {
                    var randomconverter = FancyConverters[ConverterValues[rnd.Next(0, ConverterValues.Length-1)]];
                    if (!randomconverter.Random)
                    {
                        if (randomconverter.CharacterConversions.ContainsKey(input[i]))
                            output += randomconverter.CharacterConversions[input[i]];
                        else if (!randomconverter.IgnoreMissingChars)
                            output += input[i];
                        i++;
                    }
                }
                else
                {
                    if (convert.CharacterConversions.ContainsKey(input[i]))
                        output += convert.CharacterConversions[input[i]];
                    else if (!convert.IgnoreMissingChars)
                        output += input[i];
                    i++;
                }

            }

            return output;
        }
    }
}
