using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class RubricCriterion
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("long_description")]
        public string LongDescription { get; set; }

        [JsonProperty("points")]
        public double Points { get; set; }

        [JsonProperty("criterion_use_range")]
        public bool CriterionUseRange { get; set; }

        [JsonProperty("ratings")]
        public List<RubricRating> Ratings { get; set; }
    }
}
