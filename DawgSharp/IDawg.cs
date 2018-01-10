using System.Collections.Generic;

namespace DawgSharp
{
    interface IDawg<TPayload>
    {
        TPayload this[IEnumerable<char> word] { get; }

        int GetLongestCommonPrefixLength (IEnumerable<char> word);

        /// <summary>
        /// Returns all elements with key matching given <paramref name="prefix"/>.
        /// </summary>
        IEnumerable <KeyValuePair <string, TPayload>> MatchPrefix (IEnumerable<char> prefix);

        int GetNodeCount ();
    }
}