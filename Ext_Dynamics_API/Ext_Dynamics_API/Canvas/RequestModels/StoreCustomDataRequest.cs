using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.RequestModels
{
    public class StoreCustomDataRequest
    {
        [JsonProperty("ns")]
        public string Namespace { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
