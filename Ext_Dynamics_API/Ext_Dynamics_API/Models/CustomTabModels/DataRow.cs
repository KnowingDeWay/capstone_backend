using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class DataRow
    {
        public object Value { get; set; }
        public DataColumn DataColumn { get; set; }
        public DataTableStudent AssociatedUser { get; set; } // This could for example, be the student this row is related with
        public bool ValueChanged { get; set; }
    }
}
