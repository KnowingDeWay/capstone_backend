using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.RequestModels
{
    public class EnrollUserRequest
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string UserId { get; set; }
        public string EnrollmentType { get; set; }
        public string EnrollmentState { get; set; }
    }
}
