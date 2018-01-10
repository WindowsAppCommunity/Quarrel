using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DawgSharp
{
    class MatrixDawg <TPayload> : IDawg <TPayload>
    {
        public TPayload this [IEnumerable <char> word]
        {
            get
            {
                int node_i = rootNodeIndex;

                foreach (var c in word)
                {
                    int childIndexPlusOne = GetChildIndexPlusOne(node_i, c);

                    if (childIndexPlusOne == 0) return default (TPayload);

                    node_i = childIndexPlusOne - 1;
                }

                if (node_i == -1) return default (TPayload);

                return node_i < payloads.Length ? payloads [node_i] : default (TPayload);
            }
        }

        /// <summary>
        /// Returns a series of node indices 
        /// </summary>
        IEnumerable <int> GetPath (IEnumerable<char> word)
        {
            int node_i = rootNodeIndex;

            yield return node_i;

            foreach (var c in word)
            {
                int childIndexPlusOne = GetChildIndexPlusOne(node_i, c);

                if (childIndexPlusOne == 0)
                {
                    yield return -1;
                    yield break;
                }

                node_i = childIndexPlusOne - 1;

                yield return node_i;
            }
        }

        int GetChildIndexPlusOne (int node_i, char c)
        {
            var children = node_i < payloads.Length ? children1 : children0;

            if (node_i >= payloads.Length) node_i -= payloads.Length;

            if (node_i >= children.GetLength(0)) return 0; // node has no children

            if (c < firstChar) return 0;
            if (c > lastChar) return 0;

            ushort charIndexPlusOne = charToIndexPlusOne [c - firstChar];

            if (charIndexPlusOne == 0) return 0;

            return children [node_i, charIndexPlusOne - 1];
        }

        public int GetLongestCommonPrefixLength (IEnumerable <char> word)
        {
            return GetPath (word).Count(i => i != -1) - 1;
        }

        struct StackItem
        {
            public int node_i, child_i;
        }

        public IEnumerable <KeyValuePair <string, TPayload>> MatchPrefix (IEnumerable<char> prefix)
        {
            string prefixStr = prefix as string ?? new string(prefix.ToArray());

            int node_i = prefixStr.Length == 0 ? rootNodeIndex : GetPath (prefixStr).Last();

            var stack = new Stack<StackItem>();

            if (node_i != -1)
            {
                if (node_i < payloads.Length)
                {
                    var payload = payloads [node_i];

                    if (! EqualityComparer<TPayload>.Default.Equals(payload, default (TPayload)))
                    {
                        yield return new KeyValuePair<string, TPayload> (prefixStr, payload);
                    }
                }

                var sb = new StringBuilder (prefixStr);

                int child_i = -1;

                for (;;)
                {
                    var children = node_i < payloads.Length ? children1 : children0;

                    int adj_node_i = (node_i >= payloads.Length) 
                        ? node_i - payloads.Length
                        : node_i;

                    if (adj_node_i < children.GetLength(0))
                    {
                        int next_child_i = child_i + 1;

                        for (; next_child_i < indexToChar.Length; ++next_child_i)
                        {
                            if (children [adj_node_i, next_child_i] != 0)
                            {
                                break;
                            }
                        }

                        if (next_child_i < indexToChar.Length)
                        {
                            stack.Push(new StackItem {node_i = node_i, child_i = next_child_i});
                            sb.Append(indexToChar [next_child_i]);
                            node_i = children [adj_node_i, next_child_i] - 1;

                            if (node_i < payloads.Length)
                            {
                                var payload = payloads [node_i];

                                if (! EqualityComparer<TPayload>.Default.Equals(payload, default (TPayload)))
                                {
                                    yield return new KeyValuePair<string, TPayload> (sb.ToString(), payload);
                                }
                            }

                            continue;
                        }
                    }

                    // No (more) children.

                    if (stack.Count == 0) break;

                    --sb.Length;
                    var item = stack.Pop();

                    node_i = item.node_i;
                    child_i = item.child_i;
                }
            }
        }

        public void SaveAsOldDawg (Stream stream, Action <BinaryWriter, TPayload> writePayload)
        {
            throw new NotImplementedException();
        }

        public int GetNodeCount()
        {
            return nodeCount;
        }

        private readonly TPayload[] payloads;
        private readonly int[,] children1;
        private readonly int[,] children0;
        private readonly char[] indexToChar;
        private readonly ushort[] charToIndexPlusOne;
        private readonly int nodeCount, rootNodeIndex;
        private readonly char firstChar;
        private readonly char lastChar;

        public MatrixDawg (BinaryReader reader, Func <BinaryReader, TPayload> readPayload)
        {
            // The nodes are grouped by (has payload, has children).
            nodeCount = reader.ReadInt32();

            rootNodeIndex = reader.ReadInt32();

            payloads = ReadArray(reader, readPayload);

            indexToChar = ReadArray(reader, r => r.ReadChar());

            charToIndexPlusOne = GetCharToIndexPlusOneMap(indexToChar);

            children1 = ReadChildren(reader, indexToChar);
            children0 = ReadChildren(reader, indexToChar);

            firstChar = indexToChar.FirstOrDefault();
            lastChar = indexToChar.LastOrDefault();
        }

        public static ushort[] GetCharToIndexPlusOneMap(char [] uniqueChars)
        {
            if (uniqueChars.Length == 0) return null;

            var charToIndex = new ushort [uniqueChars.Last() - uniqueChars.First() + 1];

            for (int i = 0; i < uniqueChars.Length; ++i)
            {
                charToIndex [uniqueChars [i] - uniqueChars.First()] = (ushort) (i + 1);
            }

            return charToIndex;
        }

        private static int[,] ReadChildren(BinaryReader reader, char[] indexToChar)
        {
            uint nodeCount = reader.ReadUInt32();

            var children = new int [nodeCount, indexToChar.Length];

            for (int node_i = 0; node_i < nodeCount; ++node_i)
            {
                ushort childCount = YaleDawg<TPayload>.ReadInt (reader, indexToChar.Length + 1);

                for (ushort child_i = 0; child_i < childCount; ++child_i)
                {
                    ushort charIndex = YaleDawg<TPayload>.ReadInt (reader, indexToChar.Length);
                    int childNodeIndex = reader.ReadInt32();

                    children [node_i, charIndex] = childNodeIndex + 1;
                }
            }

            return children;
        }

        public static T [] ReadArray <T> (BinaryReader reader, Func<BinaryReader, T> read)
        {
            int len = reader.ReadInt32();

            return ReadSequence(reader, read).Take(len).ToArray();
        }

        static IEnumerable<T> ReadSequence <T> (BinaryReader reader, Func<BinaryReader, T> read)
        {
            for (;;) yield return read (reader);
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns
    }
}
