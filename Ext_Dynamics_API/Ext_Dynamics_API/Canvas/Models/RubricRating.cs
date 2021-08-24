using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class RubricRating
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("criterion_id")]
        public string CriterionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("long_description")]
        public string LongDescription { get; set; }

        [JsonProperty("points")]
        public double Points { get; set; }
    }
}
