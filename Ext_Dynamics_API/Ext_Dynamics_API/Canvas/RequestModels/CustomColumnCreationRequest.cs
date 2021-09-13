using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.RequestModels
{
    public class CustomColumnCreationRequest
    {
        public string Title { get; set; }
        public int? Position { get; set; }
        public bool? Hidden { get; set; }
        public bool? TeacherNotes { get; set; }
        public bool? ReadOnly { get; set; }
    }
}
