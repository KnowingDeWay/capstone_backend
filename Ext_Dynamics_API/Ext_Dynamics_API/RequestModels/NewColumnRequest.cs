using Ext_Dynamics_API.Models.CustomTabModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.RequestModels
{
    public class NewColumnRequest
    {
        public CourseDataColumn NewColumn { get; set; }
        public string CsvFileContent { get; set; }
    }
}
