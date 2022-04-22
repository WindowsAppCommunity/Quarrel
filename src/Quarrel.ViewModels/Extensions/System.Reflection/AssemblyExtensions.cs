// Quarrel © 2022

using System.IO;
using System.Linq;

namespace System.Reflection
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="Assembly"/> type.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the contents of a specified embedded. file, as a <see cref="string"/>
        /// </summary>
        /// <param name="assembly">The target <see cref="Assembly"/> instance</param>
        /// <param name="filename">The name of the file to read</param>
        public static string ReadEmbeddedFile(this Assembly assembly, string filename)
        {
            string embeddedFilename = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(filename));
            using Stream stream = assembly.GetManifestResourceStream(embeddedFilename);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
