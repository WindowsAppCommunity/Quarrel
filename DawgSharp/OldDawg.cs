using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DawgSharp
{
    class OldDawg <TPayload> : IDawg<TPayload>
    {
        readonly Node <TPayload> root;

        public TPayload this [IEnumerable<char> word]
        {
            get
            {
                var node = FindNode (word);

                return node == null ? default (TPayload) : node.Payload;
            }
        }

        private Node <TPayload> FindNode (IEnumerable<char> word)
        {
            var node = root;

            foreach (char c in word)
            {
                node = node.GetChild (c);

                if (node == null) return null;
            }

            return node;
        }

        public int GetLongestCommonPrefixLength (IEnumerable<char> word)
        {
            var node = root;
            int len = 0; 

            foreach (char c in word)
            {
                node = node.GetChild (c);

                if (node == null) break;

                ++len;
            }

            return len;
        }

        /// <summary>
        /// Returns all elements with key matching given <paramref name="prefix"/>.
        /// </summary>
        public IEnumerable <KeyValuePair <string, TPayload>> MatchPrefix (IEnumerable<char> prefix)
        {
            var node = FindNode (prefix);

            if (node == null) return Enumerable.Empty <KeyValuePair <string, TPayload>> ();

            var sb = new StringBuilder ();

            sb.Append (prefix.ToArray ());

            return new PrefixMatcher <TPayload> (sb).MatchPrefix (node);
        }

        private class NodeByPayloadComparer : IComparer<Node<TPayload>>
        {
            public int Compare(Node<TPayload> x, Node<TPayload> y)
            {
                return - x.HasPayload.CompareTo(y.HasPayload);
            }
        }

        public void SaveAsYaleDawg (BinaryWriter writer, Action<BinaryWriter, TPayload> writePayload)
        {
            const int version = 2;
            writer.Write (version);

            var allNodes = root.GetAllDistinctNodes().ToArray();

            Array.Sort(allNodes, new NodeByPayloadComparer ());
            int totalChildCount = allNodes.Sum(n => n.Children.Count);

            writer.Write (allNodes.Length);

            var nodeIndex = allNodes
                .Select((node, i) => new {node, i})
                .ToDictionary(t => t.node, t => t.i);

            int rootNodeIndex;
            if (!nodeIndex.TryGetValue(root, out rootNodeIndex))
            {
                rootNodeIndex = -1;
            }

            writer.Write (rootNodeIndex);

            var nodesWithPayloads = allNodes.TakeWhile(n => n.HasPayload).ToArray();

            writer.Write (nodesWithPayloads.Length);

            foreach (var node in nodesWithPayloads)
            {
                writePayload (writer, node.Payload);
            }

            var allChars = allNodes.SelectMany (node => node.Children.Keys).Distinct().OrderBy(c => c).ToArray();

            writer.Write (allChars.Length);

            foreach (char c in allChars)
            {
                writer.Write (c);
            }

            writer.Write (totalChildCount);

            WriteChildrenNoLength(writer, allNodes, nodeIndex, allChars);
        }

        public void SaveAsMatrixDawg (BinaryWriter writer, Action<BinaryWriter, TPayload> writePayload)
        {
            const int version = 1;
            writer.Write (version);

            var allNodes = root.GetAllDistinctNodes()
                .ToArray();

            writer.Write (allNodes.Length);

            var cube = new Node <TPayload> [2,2] [];

            var nodeGroups = allNodes.GroupBy (node => new {node.HasPayload, node.HasChildren})
                .ToDictionary(g => g.Key, g => g.ToArray());

            for (int p = 0; p < 2;  ++p)
            for (int c = 0; c < 2;  ++c)
            {
                Node<TPayload> [] arr;
                cube [p, c] = nodeGroups.TryGetValue(new {HasPayload = p != 0, HasChildren = c != 0}, out arr) ? arr : new Node<TPayload>[0];
            }

            var nodesWithPayloads = cube [1, 1].Concat(cube [1, 0]).ToArray();

            var nodeIndex = nodesWithPayloads.Concat(cube [0, 1].Concat(cube [0, 0]))
                .Select((node, i) => new {node, i})
                .ToDictionary(t => t.node, t => t.i);

            var rootNodeIndex = nodeIndex [root];

            writer.Write (rootNodeIndex);

            writer.Write (nodesWithPayloads.Length);

            foreach (var node in nodesWithPayloads)
            {
                writePayload (writer, node.Payload);
            }

            var allChars = allNodes.SelectMany (node => node.Children.Keys).Distinct().OrderBy(c => c).ToArray();

            writer.Write (allChars.Length);

            foreach (char c in allChars)
            {
                writer.Write (c);
            }

            WriteChildren (writer, nodeIndex, cube [1, 1], allChars);
            WriteChildren (writer, nodeIndex, cube [0, 1], allChars);
        }

        private static void WriteChildren (BinaryWriter writer, Dictionary<Node<TPayload>, int> nodeIndex, Node<TPayload>[] nodes, char[] allChars)
        {
            writer.Write (nodes.Length);

            WriteChildrenNoLength(writer, nodes, nodeIndex, allChars);
        }

        private static void WriteChildrenNoLength(BinaryWriter writer, IEnumerable<Node<TPayload>> nodes, Dictionary<Node<TPayload>, int> nodeIndex, char[] allChars)
        {
            var charToIndexPlusOne = MatrixDawg<TPayload>.GetCharToIndexPlusOneMap (allChars);

            char firstChar = allChars.FirstOrDefault();

            foreach (var node in nodes)
            {
                WriteInt (writer, node.Children.Count, allChars.Length + 1);

                foreach (var child in node.Children.OrderBy(c => c.Key))
                {
                    int charIndex = charToIndexPlusOne [child.Key - firstChar] - 1;

                    WriteInt (writer, charIndex, allChars.Length);

                    writer.Write (nodeIndex [child.Value]);
                }
            }
        }

        private static void WriteInt(BinaryWriter writer, int charIndex, int countOfPossibleValues)
        {
            if (countOfPossibleValues > 256)
            {
                writer.Write ((ushort) charIndex);
            }
            else
            {
                writer.Write ((byte) charIndex);
            }
        }

        internal OldDawg (Node <TPayload> root)
        {
            this.root = root;
        }

        public int GetNodeCount ()
        {
            return root.GetRecursiveChildNodeCount ();
        }
    }
}