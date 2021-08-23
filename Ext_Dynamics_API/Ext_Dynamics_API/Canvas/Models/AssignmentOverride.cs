using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class AssignmentOverride
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("assignment_id")]
        public int AssignmentId { get; set; }

        [JsonProperty("student_ids")]
        public List<int> StudentIds { get; set; }

        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("course_section_id")]
        public int CourseSectionId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("due_at")]
        public DateTime? DueAt { get; set; }

        [JsonProperty("all_day")]
        public bool AllDay { get; set; }

        [JsonProperty("all_day_date")]
        public string AllDayDate { get; set; }

        [JsonProperty("unlock_at")]
        public DateTime? UnlockAt { get; set; }

        [JsonProperty("lock_at")]
        public DateTime? LockAt { get; set; }
    }
}
