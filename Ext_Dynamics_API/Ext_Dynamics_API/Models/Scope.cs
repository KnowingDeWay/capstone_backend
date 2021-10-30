using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
