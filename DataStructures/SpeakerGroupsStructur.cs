using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// <partial auto-generated/>
namespace BGEdit.SpeakerGroupsStructur
{
    public partial class SpeakerGroupsStructurRoot
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }

        [JsonProperty("save")]
        public Save Save { get; set; }
    }

    public partial class Save
    {
        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("region")]
        public Region Region { get; set; }
    }

    public partial class Region
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("node")]
        public RegionNode Node { get; set; }
    }

    public partial class RegionNode
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("children")]
        public Children Children { get; set; }
    }

    public partial class Children
    {
        [JsonProperty("node")]
        public NodeElement[] Node { get; set; }
    }

    public partial class NodeElement
    {
        [JsonProperty("@id")]
        public NodeId Id { get; set; }

        [JsonProperty("attribute")]
        public Attribute[] Attribute { get; set; }
    }

    public partial class Attribute
    {
        [JsonProperty("@id")]
        public String Id { get; set; }

        [JsonProperty("@type")]
        public String Type { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }
    }

    public partial class Version
    {
        [JsonProperty("@major")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Major { get; set; }

        [JsonProperty("@minor")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Minor { get; set; }

        [JsonProperty("@revision")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Revision { get; set; }

        [JsonProperty("@build")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Build { get; set; }
    }

    public partial class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }

    public enum AttributeId { Description, Name, OverwriteSpeakerUuid, Uuid };

  

    public enum NodeId { SpeakerGroup };
 

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
