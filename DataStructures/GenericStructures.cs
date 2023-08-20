using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEdit.GenericStructures
{

    public partial class TypeValPairStr
    {
        [JsonProperty("type")]
        public String type { get; set; }

        [JsonProperty("value")]
        public String value { get; set; }
    }

    public partial class TypeValPairBool
    {
        [JsonProperty("type")]
        public String type { get; set; }

        [JsonProperty("value")]
        public bool value { get; set; }
    }

    public partial class TypeValPairUInt32
    {
        [JsonProperty("type")]
        public String type { get; set; }

        [JsonProperty("value")]
        public UInt32 value { get; set; }
    }
    public partial class TypeValPairInt32
    {
        [JsonProperty("type")]
        public String type { get; set; }

        [JsonProperty("value")]
        public Int32 value { get; set; }
    }
    public partial class TypeValPairUInt8
    {
        [JsonProperty("type")]
        public String type { get; set; }

        [JsonProperty("value")]
        public byte value { get; set; }
    }
}
