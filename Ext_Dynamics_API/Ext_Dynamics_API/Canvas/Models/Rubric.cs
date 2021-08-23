using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class Rubric
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("context_id")]
        public int ContextId { get; set; }

        [JsonProperty("context_type")]
        public string ContextType { get; set; }

        [JsonProperty("points_possible")]
        public double PointsPossible { get; set; }

        [JsonProperty("reusable")]
        public bool Reusable { get; set; }

        [JsonProperty("read_only")]
        public bool ReadOnly { get; set; }

        [JsonProperty("free_form_criterion_comments")]
        public bool FreeFormCriterionComments { get; set; }

        [JsonProperty("hide_score_total")]
        public bool HideScoreTotal { get; set; }

        [JsonProperty("data")]
        public List<RubricCriterion> Data { get; set; }

        [JsonProperty("assessments")]
        public List<RubricAssessment> Assessments { get; set; }

        [JsonProperty("associations")]
        public List<RubricAssociation> Associations { get; set; }
    }
}
