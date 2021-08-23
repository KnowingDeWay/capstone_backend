using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class NeedsGradingCount
    {
        [JsonProperty("section_id")]
        public string SectionId { get; set; }

        [JsonProperty("needs_grading_count")]
        public int NeedsGradingCountNumber { get; set; }
    }
}
