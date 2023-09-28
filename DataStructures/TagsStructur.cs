using System;
using System.Collections.Generic;
using System.Globalization;
using BGEdit.GenericStructures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
// <partial auto-generated/>
namespace BGEdit.TagStructur
{
 
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
    public class Category
    {
        [JsonProperty("Category", NullValueHandling = NullValueHandling.Ignore)]
        public List<SingleCategory> category { get; set; }
    }

    public class SingleCategory
    {
        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        public TypeValPairStr Name { get; set; }
    }

   

    public class DisplayDescription
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public int version { get; set; }

        [JsonProperty("handle", NullValueHandling = NullValueHandling.Ignore)]
        public string handle { get; set; }
    }

    public class DisplayName
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public int version { get; set; }

        [JsonProperty("handle", NullValueHandling = NullValueHandling.Ignore)]
        public string handle { get; set; }
    }

    public class Header
    {
        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public int time { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string version { get; set; }
    }



    public class Regions
    {
        [JsonProperty("Tags", NullValueHandling = NullValueHandling.Ignore)]
      
        public Tags RegTags { get; set; }

        [JsonProperty("Flags", NullValueHandling = NullValueHandling.Ignore)]

        public Tags RegFlags { get; set; }

    }

    public class TagStructurRoot
    {
        [JsonProperty("save", NullValueHandling = NullValueHandling.Ignore)]
        public Save save { get; set; }
    }

    public class Save
    {
        [JsonProperty("header", NullValueHandling = NullValueHandling.Ignore)]
        public Header header { get; set; }

        [JsonProperty("regions", NullValueHandling = NullValueHandling.Ignore)]
        public Regions regions { get; set; }
    }

    public class Tags
    {
        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public TypeValPairStr Description { get; set; }

        [JsonProperty("DisplayDescription", NullValueHandling = NullValueHandling.Ignore)]
        public DisplayDescription DisplayDescription { get; set; }

        [JsonProperty("DisplayName", NullValueHandling = NullValueHandling.Ignore)]
        public DisplayName DisplayName { get; set; }

        [JsonProperty("Icon", NullValueHandling = NullValueHandling.Ignore)]
        public TypeValPairStr Icon { get; set; }

        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        public TypeValPairStr Name { get; set; }

        [JsonProperty("UUID", NullValueHandling = NullValueHandling.Ignore)]
        public TypeValPairStr UUID { get; set; }

        [JsonProperty("Categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<Category> Categories { get; set; }
    }



}
