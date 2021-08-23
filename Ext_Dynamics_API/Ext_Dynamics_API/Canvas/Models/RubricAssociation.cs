using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class RubricAssociation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rubric_id")]
        public int RubricId { get; set; }

        [JsonProperty("association_id")]
        public int AssociationId { get; set; }

        [JsonProperty("association_type")]
        public string AssociationType { get; set; }

        [JsonProperty("use_for_grading")]
        public bool UseForGrading { get; set; }

        [JsonProperty("summary_data")]
        public string SummaryData { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        [JsonProperty("hide_score_total")]
        public bool HideScoreTotal { get; set; }

        [JsonProperty("hide_points")]
        public bool HidePoints { get; set; }

        [JsonProperty("hide_outcome_results")]
        public bool HideOutcomeResults { get; set; }
    }
}
