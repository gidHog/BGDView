using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEdit.LocalizationStructur
{
    public enum DataType
    {
        Localization,
        Dialog,
        MergedChars
    }
    // Localization XML
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
