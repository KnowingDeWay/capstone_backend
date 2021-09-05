using Ext_Dynamics_API.Canvas.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.AnalysisModels
{
    public class UserCourseLevelAnalysis
    {
        [JsonProperty("assignment_id")]
        public int AssignmentId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("unlock_at")]
        public DateTime? UnlockAt { get; set; }

        [JsonProperty("points_possible")]
        public double PointsPossible { get; set; }

        [JsonProperty("non_digital_submission")]
        public bool NonDigitalSubmission { get; set; }

        [JsonProperty("multiple_due_dates")]
        public bool MultipleDueDates { get; set; }

        [JsonProperty("due_at")]
        public DateTime? DueAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("muted")]
        public bool Muted { get; set; }

        [JsonProperty("max_score")]
        public double MaxScore { get; set; }

        [JsonProperty("min_score")]
        public double MinScore { get; set; }

        [JsonProperty("first_quartile")]
        public double FirstQuartile { get; set; }

        [JsonProperty("median")]
        public double Median { get; set; }

        [JsonProperty("third_quartile")]
        public double ThirdQuartile { get; set; }

        [JsonProperty("module_ids")]
        public List<int> ModuleIds { get; set; }

        [JsonProperty("excused")]
        public bool Excused { get; set; }

        [JsonProperty("submission")]
        public Submission Submission { get; set; }
    }
}
