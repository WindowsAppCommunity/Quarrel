using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DawgSharp
{
    public class Dawg <TPayload> : IEnumerable <KeyValuePair <string, TPayload>>
    {
        readonly IDawg <TPayload> dawg;

        internal Dawg (IDawg <TPayload> dawg)
        {
            this.dawg = dawg;
        }

        public TPayload this [IEnumerable<char> word] => dawg [word];

        public int GetLongestCommonPrefixLength (IEnumerable<char> word)
        {
            return dawg.GetLongestCommonPrefixLength(word);
        }

        public IEnumerable<KeyValuePair<string, TPayload>> MatchPrefix (IEnumerable<char> prefix)
        {
            return dawg.MatchPrefix(prefix);
        }

        public int GetNodeCount ()
        {
            return dawg.GetNodeCount ();
        }

        public IEnumerator<KeyValuePair<string, TPayload>> GetEnumerator()
        {
            return MatchPrefix ("").GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator ();
        }

        /// <summary>
        /// Save the DAWG to a file / stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="writePayload">Optional, can be null for basic types (int, string, etc).</param>
        public void SaveTo (Stream stream, Action<BinaryWriter, TPayload> writePayload = null)
        {
#pragma warning disable 618
            SaveAsYaleDawg (stream, writePayload ?? GetStandardWriter ());
#pragma warning restore 618
        }

        /// <summary>
        /// This method is only used for testing.
        /// </summary>
        [Obsolete ("This method is only used for testing.")]
        public void SaveAsYaleDawg (Stream stream, Action <BinaryWriter, TPayload> writePayload = null)
        {
            Save (stream, (d, w) => d.SaveAsYaleDawg (w, writePayload ?? GetStandardWriter ()));
        }

        /// <summary>
        /// This method is only used for testing.
        /// </summary>
        [Obsolete ("This method is only used for testing.")]
        public void SaveAsMatrixDawg (Stream stream, Action <BinaryWriter, TPayload> writePayload = null)
        {
            Save (stream, (d, w) => d.SaveAsMatrixDawg (w, writePayload ?? GetStandardWriter ()));
        }

        private void Save (Stream stream, Action <OldDawg<TPayload>, BinaryWriter> save)
        {
            // Do not close the BinaryWriter. Users might want to append more data to the stream.
            var writer = new BinaryWriter (stream);

            writer.Write (GetSignature ());

            save ((OldDawg<TPayload>) dawg, writer);
        }

        static Action <BinaryWriter, TPayload> GetStandardWriter ()
        {
            object writer;
            if (!Writers.TryGetValue(typeof (TPayload), out writer))
            {
                throw new Exception("Could not find a serialization method for " + typeof(TPayload).Name + ". Use a SaveXXX overload with a 'writePayload' parameter.");
            }
            return (Action <BinaryWriter, TPayload>) writer;
        }

        static readonly Dictionary<Type, object> Writers = new Dictionary<Type, object>
        {
            {typeof (bool),    new Action <BinaryWriter, bool>    ((r, payload) => r.Write(payload))},
            {typeof (int),     new Action <BinaryWriter, int>     ((r, payload) => r.Write(payload))},
            {typeof (uint),    new Action <BinaryWriter, uint>    ((r, payload) => r.Write(payload))},
            {typeof (long),    new Action <BinaryWriter, long>    ((r, payload) => r.Write(payload))},
            {typeof (ulong),   new Action <BinaryWriter, ulong>   ((r, payload) => r.Write(payload))},
            {typeof (byte),    new Action <BinaryWriter, byte>    ((r, payload) => r.Write(payload))},
            {typeof (sbyte),   new Action <BinaryWriter, sbyte>   ((r, payload) => r.Write(payload))},
            {typeof (short),   new Action <BinaryWriter, short>   ((r, payload) => r.Write(payload))},
            {typeof (ushort),  new Action <BinaryWriter, ushort>  ((r, payload) => r.Write(payload))},
            {typeof (string),  new Action <BinaryWriter, string>  ((r, payload) => r.Write(payload))},
            {typeof (char),    new Action <BinaryWriter, char>    ((r, payload) => r.Write(payload))},
            {typeof (double),  new Action <BinaryWriter, double>  ((r, payload) => r.Write(payload))},
            {typeof (float),   new Action <BinaryWriter, float>   ((r, payload) => r.Write(payload))},
            {typeof (decimal), new Action <BinaryWriter, decimal> ((r, payload) => r.Write(payload))},
        };

        static readonly Dictionary<Type, object> Readers = new Dictionary<Type, object>
        {
            {typeof (bool),    new Func <BinaryReader, bool>    (r => r.ReadBoolean())},
            {typeof (int),     new Func <BinaryReader, int>     (r => r.ReadInt32())},
            {typeof (uint),    new Func <BinaryReader, uint>    (r => r.ReadUInt32())},
            {typeof (long),    new Func <BinaryReader, long>    (r => r.ReadInt64())},
            {typeof (ulong),   new Func <BinaryReader, ulong>   (r => r.ReadUInt64())},
            {typeof (byte),    new Func <BinaryReader, byte>    (r => r.ReadByte())},
            {typeof (sbyte),   new Func <BinaryReader, sbyte>   (r => r.ReadSByte())},
            {typeof (short),   new Func <BinaryReader, short>   (r => r.ReadInt16())},
            {typeof (ushort),  new Func <BinaryReader, ushort>  (r => r.ReadUInt16())},
            {typeof (string),  new Func <BinaryReader, string>  (r => r.ReadString())},
            {typeof (char),    new Func <BinaryReader, char>    (r => r.ReadChar())},
            {typeof (double),  new Func <BinaryReader, double>  (r => r.ReadDouble())},
            {typeof (float),   new Func <BinaryReader, float>   (r => r.ReadSingle())},
            {typeof (decimal), new Func <BinaryReader, decimal> (r => r.ReadDecimal())},
        };

        public static Dawg <TPayload> Load (Stream stream, Func <BinaryReader, TPayload> readPayload = null)
        {
            return new Dawg <TPayload> (LoadIDawg (stream, readPayload ?? (Func <BinaryReader, TPayload>) Readers [typeof(TPayload)])); 
        }

        static IDawg <TPayload> LoadIDawg (Stream stream, Func <BinaryReader, TPayload> readPayload)
        {
            using (var reader = new BinaryReader (stream))
            {
                int signature = GetSignature();

                int firstInt = reader.ReadInt32 ();

                if (firstInt == signature)
                {
                    int version = reader.ReadInt32();

                    switch (version)
                    {
                        case 1: return new MatrixDawg <TPayload> (reader, readPayload);
                        case 2: return new YaleDawg <TPayload> (reader, readPayload);
                    }

                    throw new Exception("This file was produced by a more recent version of DawgSharp.");
                }

                // The old, unversioned, file format had the number of nodes as the first 4 bytes of the stream.
                // It is extremely unlikely that they happen to be exactly the same as the signature "DAWG".
                return LoadOldDawg (reader, firstInt, readPayload);
            }
        }

        private static int GetSignature()
        {
            var bytes = Encoding.UTF8.GetBytes("DAWG");

            return bytes [0]
                + bytes [1] << 8
                + bytes [2] << 16
                + bytes [3] << 24;
        }

        private static OldDawg<TPayload> LoadOldDawg (BinaryReader reader, int nodeCount, Func<BinaryReader, TPayload> readPayload)
        {
            var nodes = new Node<TPayload>[nodeCount];

            var rootIndex = reader.ReadInt32 ();

            var chars = reader.ReadChars (nodeCount);

            for (int i = 0; i < nodeCount; ++i)
            {
                var node = new Node <TPayload> ();
                    
                int childCount = reader.ReadInt16 ();

                while (childCount --> 0)
                {
                    int childIndex = reader.ReadInt32 ();

                    node.Children.Add (chars [childIndex], nodes [childIndex]);
                }

                node.Payload = readPayload (reader);

                nodes [i] = node;
            }

            return new OldDawg <TPayload> (nodes [rootIndex]);
        }
    }
}