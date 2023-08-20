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
        public TypeEnum Type { get; set; }

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

    public enum TypeEnum { LsString };

    public enum NodeId { SpeakerGroup };

    public partial class SpeakerGroupsStructurRoot
    {
        public static SpeakerGroupsStructurRoot FromJson(string json) => JsonConvert.DeserializeObject<SpeakerGroupsStructurRoot>(json, BGEdit.SpeakerGroupsStructur.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SpeakerGroupsStructurRoot self) => JsonConvert.SerializeObject(self, BGEdit.SpeakerGroupsStructur.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                NodeIdConverter.Singleton,
                AttributeIdConverter.Singleton,
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class NodeIdConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(NodeId) || t == typeof(NodeId?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "SpeakerGroup")
            {
                return NodeId.SpeakerGroup;
            }
            throw new Exception("Cannot unmarshal type NodeId");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (NodeId)untypedValue;
            if (value == NodeId.SpeakerGroup)
            {
                serializer.Serialize(writer, "SpeakerGroup");
                return;
            }
            throw new Exception("Cannot marshal type NodeId");
        }

        public static readonly NodeIdConverter Singleton = new NodeIdConverter();
    }

    internal class AttributeIdConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AttributeId) || t == typeof(AttributeId?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Description":
                    return AttributeId.Description;
                case "Name":
                    return AttributeId.Name;
                case "OverwriteSpeakerUuid":
                    return AttributeId.OverwriteSpeakerUuid;
                case "UUID":
                    return AttributeId.Uuid;
            }
            throw new Exception("Cannot unmarshal type AttributeId");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AttributeId)untypedValue;
            switch (value)
            {
                case AttributeId.Description:
                    serializer.Serialize(writer, "Description");
                    return;
                case AttributeId.Name:
                    serializer.Serialize(writer, "Name");
                    return;
                case AttributeId.OverwriteSpeakerUuid:
                    serializer.Serialize(writer, "OverwriteSpeakerUuid");
                    return;
                case AttributeId.Uuid:
                    serializer.Serialize(writer, "UUID");
                    return;
            }
            throw new Exception("Cannot marshal type AttributeId");
        }

        public static readonly AttributeIdConverter Singleton = new AttributeIdConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "LSString")
            {
                return TypeEnum.LsString;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            if (value == TypeEnum.LsString)
            {
                serializer.Serialize(writer, "LSString");
                return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
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
