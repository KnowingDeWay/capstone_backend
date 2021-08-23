using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class RubricAssessment
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rubric_id")]
        public int RubricId { get; set; }

        [JsonProperty("rubric_association_id")]
        public int RubricAssociationId { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("artifact_type")]
        public string ArtifactType { get; set; }

        [JsonProperty("artifact_id")]
        public int ArtifactId { get; set; }

        [JsonProperty("artifact_attempt")]
        public int ArtifactAttempt { get; set; }

        [JsonProperty("assessment_type")]
        public string AssessmentType { get; set; }

        [JsonProperty("assessor_id")]
        public int AssessorId { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("comments")]
        public object Comments { get; set; }
    }
}
