using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class RubricSettings
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("points_possible")]
        public double PointsPossible { get; set; }

        [JsonProperty("free_form_criterion_comments")]
        public bool FreeFormCriterionComments { get; set; }

        [JsonProperty("hide_score_total")]
        public bool HideScoreTotal { get; set; }

        [JsonProperty("hide_points")]
        public bool HidePoints { get; set; }
    }
}
