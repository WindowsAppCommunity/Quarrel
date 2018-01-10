using System.Collections.Generic;

namespace DawgSharp
{
    class NodeWrapperEqualityComparer <TPayload> : IEqualityComparer <NodeWrapper <TPayload>>
    {
        public bool Equals (NodeWrapper <TPayload> x, NodeWrapper <TPayload> y)
        {
            var equals = EqualityComparer<TPayload>.Default.Equals (x.Node.Payload, y.Node.Payload)
                          && SequenceEqual(x.Node.SortedChildren, y.Node.SortedChildren);

            return equals;
        }

        private static bool SequenceEqual(
            IEnumerable<KeyValuePair<char, Node<TPayload>>> x, 
            IEnumerable<KeyValuePair<char, Node<TPayload>>> y)
        {
            // Do not bother disposing of these enumerators.

            // ReSharper disable GenericEnumeratorNotDisposed
            var xe = x.GetEnumerator();
            var ye = y.GetEnumerator();
            // ReSharper restore GenericEnumeratorNotDisposed

            while (xe.MoveNext())
            {
                if (!ye.MoveNext()) return false;

                var xcurrent = xe.Current;
                var ycurrent = ye.Current;

                if (xcurrent.Key != ycurrent.Key) return false;
                if (!Equals(xcurrent.Value, ycurrent.Value)) return false;
            }

            return !ye.MoveNext();
        }

        static int GetHashCode (Node<TPayload> node)
        {
            int hashCode = EqualityComparer<TPayload>.Default.GetHashCode (node.Payload);

            foreach (var c in node.Children)
            {
                hashCode ^= c.Key ^ c.Value.GetHashCode();
            }

            return hashCode;
        }

        public int GetHashCode (NodeWrapper <TPayload> obj)
        {
            return GetHashCode (obj.Node);
        }
    }
}