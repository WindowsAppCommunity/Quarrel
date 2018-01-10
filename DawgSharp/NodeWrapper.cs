using System.Collections.Generic;
using System.Linq;

namespace DawgSharp
{
    class NodeWrapper <TPayload>
    {
        public NodeWrapper (Node<TPayload> node, Node<TPayload> super, char @char)
        {
            this.Node = node;
            this.Super = super;
            this.Char = @char;
        }

        public readonly Node<TPayload> Node;
        public readonly Node<TPayload> Super;
        public readonly char Char;
        KeyValuePair<char, Node<TPayload>> [] sortedChildren;

        private KeyValuePair<char, Node<TPayload>>[] GetSortedChildren()
        {
            return Node.Children.OrderBy(c => c.Key).ToArray();
        }

        public KeyValuePair<char, Node<TPayload>> [] SortedChildren => sortedChildren ?? (sortedChildren = GetSortedChildren());
    }
}