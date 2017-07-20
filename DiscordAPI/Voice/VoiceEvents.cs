using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public struct VoiceEvent
    {
        [JsonProperty("op")]
        public int? Operation { get; set; }
        [JsonProperty("d")]
        public object Data { get; set; }
        [JsonProperty("s")]
        public int? SequenceNumber { get; set; }
        [JsonProperty("t")]
        public string Type { get; set; }

        public T GetData<T>()
        {
            var dataAsJObject = Data as JObject;
            return dataAsJObject.ToObject<T>();
        }
    }

    //public struct VoicePacketHeaderStructure //This may be bullshit
    //{
    //    [JsonProperty("type")]
    //    public byte Type { get; set; }
    //    [JsonProperty("version")]
    //    public byte Version { get; set; }
    //    [JsonProperty("padding")]
    //    public byte Padding { get; set; }
    //    [JsonProperty("extension")]
    //    public byte Extension { get; set; }
    //    [JsonProperty("csrc")]
    //    public byte CSRC { get; set; }
    //    [JsonProperty("sequence")]
    //    public ushort Sequence { get; set; }
    //    [JsonProperty("timestamp")]
    //    public uint Timestamp { get; set; }
    //    [JsonProperty("ssrc")]
    //    public uint SSRC { get; set; }

    //    public T GetData<T>()
    //    {
    //        var dataAsJObject = Data as JObject;
    //        return dataAsJObject.ToObject<T>();
    //    }
    //}
}
