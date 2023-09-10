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
        public long Major { get; set; }

        [JsonProperty("@minor")]
        public long Minor { get; set; }

        [JsonProperty("@revision")]
        public long Revision { get; set; }

        [JsonProperty("@build")]
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
        public static TagStructurRoot FromJson(string json) => JsonConvert.DeserializeObject<TagStructurRoot>(json);
    }
   

}
