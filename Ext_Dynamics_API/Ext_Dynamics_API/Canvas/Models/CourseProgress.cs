using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class CourseProgress
    {
       [JsonProperty("requirement_count")]
       public int? RequirementCount { get; set; }

       [JsonProperty("requirement_completed_count")]
       public int? RequirementCompletedCount { get; set; }

       [JsonProperty("next_requirement_url")]
       public string NextRequirementUrl { get; set; }

       [JsonProperty("completed_at")]
       public DateTime? CompletedAt { get; set; }
    }
}
