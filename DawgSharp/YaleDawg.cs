using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DawgSharp
{
    class YaleDawg <TPayload> : IDawg<TPayload>
    {
        struct Child
        {
            public readonly int Index;
            public readonly ushort CharIndex;

            public Child(int index, ushort charIndex)
            {
                Index = index;
                CharIndex = charIndex;
            }
        }

        private readonly TPayload[] payloads;
        private readonly char[] indexToChar;
        private readonly ushort[] charToIndexPlusOne;
        private readonly int nodeCount, rootNodeIndex;
        private readonly char firstChar;
        private readonly char lastChar;
        private readonly int [] firstChildForNode;
        private readonly Child [] children; // size = NNZ

        public YaleDawg (BinaryReader reader, Func <BinaryReader, TPayload> readPayload)
        {
            // The nodes are grouped by (has payload, has children).
            nodeCount = reader.ReadInt32();

            rootNodeIndex = reader.ReadInt32();

            payloads = MatrixDawg<TPayload>.ReadArray(reader, readPayload);

            indexToChar = MatrixDawg<TPayload>.ReadArray(reader, r => r.ReadChar());

            charToIndexPlusOne = MatrixDawg<TPayload>.GetCharToIndexPlusOneMap(indexToChar);

            firstChildForNode = new int[nodeCount+1];

            int firstChildForNode_i = 0;

            int totalChildCount = reader.ReadInt32();

            children = new Child [totalChildCount];

            firstChildForNode [nodeCount] = totalChildCount;

            int globalChild_i = 0;

            for (int child1_i = 0; child1_i < nodeCount; ++child1_i)
            {
                firstChildForNode [firstChildForNode_i++] = globalChild_i;

                ushort childCount = ReadInt (reader, indexToChar.Length + 1);

                for (ushort child_i = 0; child_i < childCount; ++child_i)
                {
                    ushort charIndex = ReadInt(reader, indexToChar.Length);
                    int childNodeIndex = reader.ReadInt32();

                    children [globalChild_i++] = new Child(childNodeIndex, charIndex);
                }
            }

            firstChar = indexToChar.FirstOrDefault();
            lastChar = indexToChar.LastOrDefault();
        }

        internal static ushort ReadInt (BinaryReader reader, int countOfPossibleValues)
        {
            return countOfPossibleValues > 256 ? reader.ReadUInt16() : reader.ReadByte();
        }

        class ChildComparer : IComparer<Child>
        {
            public int Compare(Child x, Child y)
            {
                return x.CharIndex.CompareTo(y.CharIndex);
            }
        }

        static readonly ChildComparer childComparer = new ChildComparer();

        TPayload IDawg<TPayload>.this[IEnumerable<char> word]
        {
            get
            {
                int node_i = GetPath(word).Last();

                if (node_i == -1) return default (TPayload);

                return GetPayload(node_i);
            }
        }

        private TPayload GetPayload(int node_i)
        {
            return node_i < payloads.Length ? payloads [node_i] : default (TPayload);
        }

        IEnumerable<int> GetPath (IEnumerable<char> word)
        {
            int node_i = rootNodeIndex;

            if (node_i == -1)
            {
                yield return -1;
                yield break;
            }
 
            yield return node_i;

            foreach (char c in word)
            {
                if (c < firstChar || c > lastChar)
                {
                    yield return -1;
                    yield break;
                }
 
                int firstChild_i = firstChildForNode [node_i];

                int lastChild_i = firstChildForNode [node_i + 1];

                ushort charIndexPlusOne = charToIndexPlusOne [c - firstChar];

                if (charIndexPlusOne == 0)
                {
                    yield return -1;
                    yield break;
                }

                var nChildren = lastChild_i - firstChild_i;

                var charIndex = (ushort) (charIndexPlusOne - 1);

                int child_i;
                if (nChildren == 1)
                {
                    child_i = children [firstChild_i].CharIndex == charIndex ? firstChild_i : -1;
                }
                else
                {
                    var searchValue = new Child(-1, charIndex);

                    child_i = Array.BinarySearch(children, firstChild_i, nChildren, searchValue, childComparer);
                }

                if (child_i < 0)
                {
                    yield return -1;
                    yield break;
                }

                node_i = children [child_i].Index;

                yield return node_i;
            }
        }

        int IDawg<TPayload>.GetLongestCommonPrefixLength(IEnumerable<char> word)
        {
            return GetPath (word).Count(i => i != -1) - 1; // -1 for root node
        }

        IEnumerable<KeyValuePair<string, TPayload>> IDawg<TPayload>.MatchPrefix(IEnumerable<char> prefix)
        {
            string prefixStr = prefix as string ?? new string(prefix.ToArray());

            int node_i = GetPath (prefixStr).Last();

            var sb = new StringBuilder (prefixStr);

            return MatchPrefix(sb, node_i);
        }

        private IEnumerable<KeyValuePair<string, TPayload>> MatchPrefix (StringBuilder sb, int node_i)
        {
            if (node_i != -1)
            {
                var payload = GetPayload(node_i);

                if (!EqualityComparer<TPayload>.Default.Equals(payload, default (TPayload)))
                {
                    yield return new KeyValuePair<string, TPayload>(sb.ToString(), payload);
                }

                int firstChild_i = firstChildForNode [node_i];

                int lastChild_i = node_i + 1 < nodeCount
                                        ? firstChildForNode[node_i + 1]
                                        : children.Length;

                for (int i = firstChild_i; i < lastChild_i; ++i)
                {
                    var child = children [i];

                    sb.Append (indexToChar [child.CharIndex]);

                    foreach (var pair in MatchPrefix (sb, child.Index))
                    {
                        yield return pair;
                    }

                    --sb.Length;
                }
            }
        }

        int IDawg<TPayload>.GetNodeCount()
        {
            return nodeCount;
        }
    }
}