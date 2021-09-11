using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class StringDataRow : DataRowBase
    {
        public string Value { get; set; }
        public string NewValue { get; set; }
    }
}
