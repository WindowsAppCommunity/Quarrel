using System.Collections.Generic;
using System.Linq;

namespace DawgSharp
{
    class LevelBuilder <TPayload>
    {
        public static void BuildLevelsExcludingRoot (Node <TPayload> root)
        {
            var levels = new List <Dictionary <NodeWrapper <TPayload>, NodeWrapper <TPayload>>> ();

            var stack = new Stack <StackNode <TPayload>> ();

            Push (stack, root);

            while (stack.Count > 0)
            {
                if (stack.Peek().ChildIterator.MoveNext ())
                {
                    // go deeper
                    Push (stack, stack.Peek().ChildIterator.Current.Value);
                }
                else
                {
                    var current = stack.Pop ();

                    if (stack.Count > 0)
                    {
                        var parent = stack.Peek ();

                        int level = current.Level;

                        if (levels.Count <= level)
                        {
                            levels.Add (new Dictionary <NodeWrapper <TPayload>, NodeWrapper <TPayload>> (new NodeWrapperEqualityComparer<TPayload> ()));
                        }

                        var dictionary = levels [level];

                        var nodeWrapper = new NodeWrapper <TPayload> (current.Node, parent.Node, parent.ChildIterator.Current.Key);

                        NodeWrapper <TPayload> existing;
                        if (dictionary.TryGetValue (nodeWrapper, out existing))
                        {
                            parent.Node.Children [parent.ChildIterator.Current.Key] = existing.Node;
                        }
                        else
                        {
                            dictionary.Add (nodeWrapper, nodeWrapper);
                        }

                        int parentLevel = current.Level + 1;

                        if (parent.Level < parentLevel)
                        {
                            parent.Level = parentLevel;
                        }
                    }
                }
            }
        }

        private static void Push (Stack <StackNode <TPayload>> stack, Node <TPayload> node)
        {
            stack.Push (new StackNode <TPayload> {Node = node, ChildIterator = node.Children.ToList ().GetEnumerator ()});
        }
    }

    class StackNode <TPayload>
    {
        public Node <TPayload> Node;
        public IEnumerator <KeyValuePair <char, Node <TPayload>>> ChildIterator;
        public int Level;
    }
}