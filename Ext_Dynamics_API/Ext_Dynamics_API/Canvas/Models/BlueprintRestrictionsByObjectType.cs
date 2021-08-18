using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class BlueprintRestrictionsByObjectType
    {
        [JsonProperty("assignment")]
        public BlueprintRestrictions Assignment { get; set; }

        [JsonProperty("wiki_page")]
        public BlueprintRestrictions WikiPage { get; set; }
    }
}
