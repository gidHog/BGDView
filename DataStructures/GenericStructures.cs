using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BGEdit.GenericStructures
{

    public partial class TypeValPairStr
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public String Value { get; set; }
        public TypeValPairStr() { }
        public TypeValPairStr(String type, String value) { 
            this.Type = type;
            this.Value = value;
        }
       
    }
    public partial class TypeValPairFloat
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public float Value { get; set; }
        public TypeValPairFloat() { }
        public TypeValPairFloat(String type, float value)
        {
            this.Type = type;
            this.Value = value;
        }

    }
    public partial class TypeValPairBool
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public bool Value { get; set; }
        public TypeValPairBool() { }
        public TypeValPairBool(String type, bool value) {
            this.Type=type;
            this.Value=value;
        }  

    }

    public partial class TypeValPairUInt32
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public UInt32 Value { get; set; }
        public TypeValPairUInt32() { }
        public TypeValPairUInt32(String type, UInt32 value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
    public partial class TypeValPairInt32
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public Int32 Value { get; set; }
        public TypeValPairInt32() { }
        public TypeValPairInt32(String type, Int32 value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
    public partial class TypeValPairUInt16
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public ushort Value { get; set; }
        public TypeValPairUInt16() { }
        public TypeValPairUInt16(String type, ushort value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
    public partial class TypeValPairUInt8
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public byte Value { get; set; }
        public TypeValPairUInt8() { }
        public TypeValPairUInt8(String type, byte value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}
