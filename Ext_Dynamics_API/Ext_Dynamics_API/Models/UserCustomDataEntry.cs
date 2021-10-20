using Ext_Dynamics_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ext_Dynamics_API.Models
{
    public class UserCustomDataEntry : EntityBase
    {
        public string ItemName { get; set; } // Must be unique, so it is an alternate key
        public string Content { get; set; }
        public CustomDataType DataType { get; set; }
        public string Scope { get; set; }
        public int CanvasUserId { get; set; }
        public int CourseId { get; set; }
    }
}
