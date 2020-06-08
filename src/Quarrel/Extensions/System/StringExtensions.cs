// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Security.Cryptography;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the <see langword="string"/> type.
    /// </summary>
    internal static partial class StringExtensions
    {
        // The regex to identify invalid characters in a filename
        [CanBeNull]
        private static Regex _filenameRegex;

        private static Regex _xmlRegex;

        /// <summary>
        /// Gets the mapping for HTML characters that needs to be unescaped.
        /// </summary>
        [NotNull]
        private static IReadOnlyDictionary<string, string> EscapedCharactersMapping { get; } = new Dictionary<string, string>
        {
            { "&lt;", "<" },
            { "&gt;", ">" },
            { "&amp;", "&" },
            { "&#39;", "\'" },
            { "&quot;", "\"" },
        };

        /// <summary>
        /// Gets the <see cref="Regex"/> to use to unescape HTML text.
        /// </summary>
        [NotNull]
        private static Regex XmlRegex
        {
            get
            {
                if (_xmlRegex == null)
                {
                    string pattern = string.Join("|", EscapedCharactersMapping.Keys.Select(Regex.Escape));
                    _xmlRegex = new Regex(pattern);
                }

                return _xmlRegex;
            }
        }

        /// <summary>
        /// Calculates the Bernstein hash fo the input text.
        /// </summary>
        /// <param name="source">The text to hash.</param>
        /// <returns>The Bernstein has of <paramref name="source"/>.</returns>
        [Pure]
        public static unsafe int BernsteinHash([NotNull] this string source)
        {
            int result = 17;
            fixed (char* p = source)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    unchecked
                    {
                        result = p[i] + (result << 4) - result;
                    }
                }
            }

            // Return a positive value
            return result > 0 ? result : -result;
        }

        /// <summary>
        /// Compute the MD5 hash of the input <see langword="string"/>.
        /// </summary>
        /// <param name="source">The input <see langword="string"/> to hash.</param>
        /// <returns>The MD5 Hash of <paramref name="source"/>.</returns>
        [Pure]
        [NotNull]
        public static string MD5Hash([NotNull] this string source)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[]
                    input = Encoding.UTF8.GetBytes(source),
                    output = md5.ComputeHash(input);
                return CryptographicBuffer.EncodeToBase64String(output.AsBuffer());
            }
        }

        /// <summary>
        /// Returns a navigation <see cref="Uri"/> from the input URL, adding the https:// prefix if needed.
        /// </summary>
        /// <param name="url">The input URL to read.</param>
        /// <returns><paramref name="url"/> as a <see cref="Uri"/>.</returns>
        [Pure]
        [NotNull]
        public static Uri ToWebUri([NotNull] this string url)
        {
            string address = url.StartsWith("https://") ? url : $"https://{url}";
            return new Uri(address);
        }

        /// <summary>
        /// Checks whether or not the input text is a valid web URL.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if <paramref name="text"/> is a URL.</returns>
        [Pure]
        public static bool IsUrl([NotNull] this string text)
        {
            Match match = Regex.Match(text, @"^(https?:\/\/)?(?:www\.)?([^:\/\s]+)(\/.+)?$");
            if (!match.Success)
            {
                return false;
            }

            if (match.Groups[2].Value.Split(new char['.'], StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                return false;
            }

            string url = match.Groups[1].Success ? text : $"https://{text}";
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        /// <summary>
        /// Checks if the given text represents a valid IPv4 address.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if <paramref name="text"/> is an IPv4 Address.</returns>
        [Pure]
        public static bool IsIPv4Address([NotNull] this string text)
        {
            Match match = Regex.Match(text, @"^(?:https?:\/\/)?((?:\d{1,3}\.){3}\d{1,3})(?::\d+)?(?:\/|(?:\/[\w\.]+)+\/?)?$");
            return match.Success &&
                   IPAddress.TryParse(match.Groups[1].Value, out IPAddress address) &&
                   address.AddressFamily == AddressFamily.InterNetwork;
        }

        /// <summary>
        /// Checks if the given text represents a valid IPv6 address.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if <paramref name="text"/> is an IPv6 Address.</returns>
        [Pure]
        public static bool IsIPv6Address([NotNull] this string text)
        {
            Match match = Regex.Match(text, @"^(?:https?:\/\/)?((?:[A-Fa-f0-9]{1,4}:){7}[A-Fa-f0-9]{1,4})(?::\d+)?(?:\/|(?:\/[\w\.]+)+\/?)?$");
            return match.Success &&
                   IPAddress.TryParse(match.Groups[1].Value, out IPAddress address) &&
                   address.AddressFamily == AddressFamily.InterNetworkV6;
        }

        /// <summary>
        /// Unescapes the input HTML text to plain text.
        /// </summary>
        /// <param name="text">The input HTML text to unescape.</param>
        /// <returns><paramref name="text"/> as a plain text.</returns>
        [Pure]
        [NotNull]
        public static string HtmlUnescape([NotNull] this string text) => XmlRegex.Replace(text, match => EscapedCharactersMapping[match.Value]);

        /// <summary>
        /// Checks whether or not the input <see cref="string"/> is empty.
        /// </summary>
        /// <param name="text">The input <see cref="string"/> to check.</param>
        /// <returns>True if <paramref name="text"/> is <see langword="null"/> or empty.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("null => true")]
        public static bool IsNullOrEmpty([CanBeNull] this string text)
        {
            // Base checks
            if (text == null)
            {
                return true;
            }

            if ((uint)text.Length <= 0u)
            {
                return true;
            }

            if (text.Length == 1 && text[0] == 8203)
            {
                return true; // Non-breaking space
            }

            // Whitespaces check
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes all the invalid filename characters from the input <see cref="string"/>.
        /// </summary>
        /// <param name="name">The input filename.</param>
        /// <returns><paramref name="name"/> as a valid filename.</returns>
        [Pure]
        [NotNull]
        public static string ToValidFilename([NotNull] this string name)
        {
            // Build the regular expression
            if (_filenameRegex == null)
            {
                char[] invalid = Path.GetInvalidFileNameChars();
                string pattern = string.Join("|", invalid.Select(c => $@"\{c}"));
                _filenameRegex = new Regex(pattern, RegexOptions.Compiled);
            }

            return _filenameRegex.Replace(name, string.Empty);
        }
    }
}
