using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class Submission
    {
        [JsonProperty("assignment_id")]
        public int AssignmentId { get; set; }

        [JsonProperty("assignment")]
        public Assignment Assignment { get; set; }

        [JsonProperty("course")]
        public Course Course { get; set; }

        [JsonProperty("attempt")]
        public int Attempt { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("grade_matches_current_submission")]
        public bool GradeMatchesCurrentSubmission { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("score")]
        public double? Score { get; set; }

        [JsonProperty("submission_comments")]
        public List<SubmissionComment> SubmissionComments { get; set; }

        [JsonProperty("submission_type")]
        public string SubmissionType { get; set; }

        [JsonProperty("submitted_at")]
        public DateTime? SubmittedAt { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("grader_id")]
        public int GraderId { get; set; }

        [JsonProperty("graded_at")]
        public DateTime? GradedAt { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("late")]
        public bool Late { get; set; }

        [JsonProperty("assignment_visible")]
        public bool AssignmentVisible { get; set; }

        [JsonProperty("excused")]
        public bool Excused { get; set; }

        [JsonProperty("missing")]
        public bool Missing { get; set; }

        [JsonProperty("late_policy_status")]
        public string LatePolicyStatus { get; set; }

        [JsonProperty("points_deducted")]
        public double PointsDeducted { get; set; }

        [JsonProperty("seconds_late")]
        public int SecondsLate { get; set; }

        [JsonProperty("workflow_state")]
        public string WorkflowState { get; set; }

        [JsonProperty("extra_attempts")]
        public int ExtraAttempts { get; set; }

        [JsonProperty("anonymous_id")]
        public string AnonymousId { get; set; }

        [JsonProperty("posted_at")]
        public DateTime? PostedAt { get; set; }

        [JsonProperty("read_status")]
        public string ReadStatus { get; set; }
    }
}
