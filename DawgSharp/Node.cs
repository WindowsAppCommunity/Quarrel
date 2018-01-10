using System.Collections.Generic;
using System.Linq;

namespace DawgSharp
{
    class Node <TPayload>
    {
        readonly Dictionary<char, Node<TPayload>> children = new Dictionary<char, Node<TPayload>>();

        public TPayload Payload { get; set; }

        public Node<TPayload> GetOrAddEdge (char c)
        {
            Node<TPayload> newNode;

            if (! children.TryGetValue (c, out newNode))
            {
                newNode = new Node<TPayload>();

                children.Add (c, newNode);
            }

            return newNode;
        }

        public Node<TPayload> GetChild (char c)
        {
            Node<TPayload> node;

            children.TryGetValue (c, out node);

            return node;
        }

        public bool HasChildren => children.Count > 0;

        public Dictionary<char, Node<TPayload>> Children => children;

        public IOrderedEnumerable<KeyValuePair<char, Node<TPayload>>> SortedChildren 
            => children.OrderBy(e => e.Key);

        public int GetRecursiveChildNodeCount () 
            => GetAllDistinctNodes ().Count ();

        public IEnumerable<Node<TPayload>> GetAllDistinctNodes ()
        {
            var visitedNodes = new HashSet<Node<TPayload>> {this};

            var stack = new Stack <IEnumerator<KeyValuePair<char, Node<TPayload>>>> ();

            var enumerator = this.children.GetEnumerator();

            stack.Push(enumerator);

            for (;;)
            {
                if (stack.Peek().MoveNext())
                {
                    var node = stack.Peek().Current.Value;
                    if (visitedNodes.Contains(node))
                    {
                        continue;
                    }
                    visitedNodes.Add(node);
                    stack.Push (node.children.GetEnumerator());
                }
                else
                {
                    stack.Pop();
                    if (stack.Count == 0) break;
                }
            }

            return visitedNodes;
        }

        public bool HasPayload => !EqualityComparer<TPayload>.Default.Equals(Payload, default (TPayload));
    }
}