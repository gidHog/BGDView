using Newtonsoft.Json;
using System.Collections.Generic;


namespace BGEdit.LocalizationStructur
{
    public class Content
    {
        [JsonProperty("@contentuid")]
        public string contentuid { get; set; }

        [JsonProperty("@version")]
        public string version { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class ContentList
    {
        public List<Content> content { get; set; }
    }

    public class Root
    {
        [JsonProperty("?xml")]
        public Xml xml { get; set; }
        public ContentList contentList { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string version { get; set; }

        [JsonProperty("@encoding")]
        public string encoding { get; set; }
    }

}
