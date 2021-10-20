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
        public string Namespace { get; set; }
        public int CanvasUserId { get; set; }
        public int CourseId { get; set; }
    }
}
