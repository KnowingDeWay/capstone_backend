using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class ExternalToolTagAttributes
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("new_tab")]
        public bool NewTab { get; set; }

        [JsonProperty("resource_link_id")]
        public string ResourceLinkId { get; set; }
    }
}
