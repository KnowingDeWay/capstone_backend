using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ext_Dynamics_API.Models
{
    public class Scope : EntityBase
    {
        public string Name { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUserAccount User { get; set; }
        public List<UserCustomDataEntry> CustomDataEntries { get; set; }
        public int CourseId { get; set; }
        public int CanvasStudentId { get; set; } // The id of the student on Canvas for which this scope is relevant to
    }
}
