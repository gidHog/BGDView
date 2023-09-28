using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEdit.Utils
{
    public struct SearchResultData
    {

        public SearchResultData(String uuid, Dictionary<String, String> results) {

            this.UUID = uuid;
            this.ResultStrings = results;
        }

        public String UUID{ get; }
        public Dictionary<String,String> ResultStrings { get; set; } = new();

        public override string ToString() {
            String result = $"UUID: ({UUID})";
            foreach( var key in ResultStrings.Keys )
            {
                result += $"Key: {key} Value: {ResultStrings[key]}\n";
            }
            return result;
        }

    }
}
