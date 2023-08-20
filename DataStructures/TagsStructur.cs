using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
// <partial auto-generated/>
namespace BGEdit.TagStructur
{
   

    public partial class TagStructurRoot
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }

        [JsonProperty("save")]
        public Save Save { get; set; }


        public Dictionary<String, AttributeElement> attributes { get; set; } = new Dictionary<String,AttributeElement>();
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

        [JsonProperty("attribute")]
        public AttributeElement[] Attribute { get; set; }

        [JsonProperty("children")]
        public NodeChildren Children { get; set; }
    }

    public partial class AttributeElement
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty("@handle", NullValueHandling = NullValueHandling.Ignore)]
        public string Handle { get; set; }

        [JsonProperty("@version", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Version { get; set; }
    }

    public partial class NodeChildren
    {
        [JsonProperty("node")]
        [JsonConverter(typeof(SingleOrArrayConverter<ChildrenNode>))]
        public List<ChildrenNode> Node { get; set; }
    }

    public partial class ChildrenNode
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("children")]
        public FluffyChildren Children { get; set; }
    }
    //copy from https://stackoverflow.com/questions/18994685/how-to-handle-both-a-single-item-and-an-array-for-the-same-property-using-json-n
    class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            if (token.Type == JTokenType.Null)
            {
                return null;
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public partial class FluffyChildren
    {
        [JsonProperty("node")]
        [JsonConverter(typeof(SingleOrArrayConverter<NodeElement>))]
        public List<NodeElement> Node { get; set; }

    }

    public partial class NodeElement
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("attribute")]
        public PurpleAttribute Attribute { get; set; }
        [JsonProperty("children")]
        public FluffyChildren Children { get; set; }
    }

    public partial class PurpleAttribute
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

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

    public partial class TagStructurRoot
    {
        public static TagStructurRoot FromJson(string json) => JsonConvert.DeserializeObject<TagStructurRoot>(json, BGEdit.TagStructur.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this TagStructurRoot self) => JsonConvert.SerializeObject(self, BGEdit.TagStructur.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

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
