using System.Collections.Generic;
using System.Text;

namespace DawgSharp
{
    class PrefixMatcher <TPayload>
    {
        readonly StringBuilder sb;

        public PrefixMatcher (StringBuilder sb)
        {
            this.sb = sb;
        }

        public IEnumerable<KeyValuePair <string, TPayload>> MatchPrefix (Node <TPayload> node)
        {
            if (!EqualityComparer <TPayload>.Default.Equals (node.Payload, default (TPayload)))
            {
                yield return new KeyValuePair <string, TPayload> (sb.ToString (), node.Payload);
            }

            foreach (var child in node.Children)
            {
                sb.Append (child.Key);
                
                foreach (var kvp in MatchPrefix(child.Value))
                {
                    yield return kvp;
                }

                --sb.Length;
            }
        }
    }
}