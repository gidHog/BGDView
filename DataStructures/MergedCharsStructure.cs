using System.Collections.Generic;
using Newtonsoft.Json;
using BGEdit.TagStructur;
// <partial auto-generated/>
namespace BGEdit.MergedCharsStructure
{
    
    public class Attribute
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }

        [JsonProperty("@value")]
        public string value { get; set; }

        [JsonProperty("@handle")]
        public string handle { get; set; }

        [JsonProperty("@version")]
        public string version { get; set; }
    }

    public class Children
    {
        [JsonConverter(typeof(SingleOrArrayConverter<Node>))]
        public List<Node> node { get; set; }
    }

    public class Node
    {
        [JsonProperty("@id")]
        public string id { get; set; }
        public Children children { get; set; }
        [JsonConverter(typeof(SingleOrArrayConverter<Attribute>))]
        public List<Attribute> attribute { get; set; }
    }

    public class Region
    {
        [JsonProperty("@id")]
        public string id { get; set; }
        public Node node { get; set; }
    }

    public class Root
    {
        [JsonProperty("?xml")]
        public Xml xml { get; set; }
        public Save save { get; set; }
    }

    public class Save
    {
        public Version version { get; set; }
        public Region region { get; set; }
    }

    public class Version
    {
        [JsonProperty("@major")]
        public string major { get; set; }

        [JsonProperty("@minor")]
        public string minor { get; set; }

        [JsonProperty("@revision")]
        public string revision { get; set; }

        [JsonProperty("@build")]
        public string build { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string version { get; set; }

        [JsonProperty("@encoding")]
        public string encoding { get; set; }
    }


}



