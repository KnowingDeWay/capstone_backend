using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class BlueprintRestrictions
    {
        [JsonProperty("content")]
        public bool Content { get; set; }

        [JsonProperty("points")]
        public bool Points { get; set; }

        [JsonProperty("due_dates")]
        public bool DueDates { get; set; }

        [JsonProperty("availability_dates")]
        public bool AvailabilityDates { get; set; }
    }
}
