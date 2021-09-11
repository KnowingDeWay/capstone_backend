using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class NumericDataRow : DataRowBase
    {
        public double Value { get; set; }
        public double NewValue { get; set; }
    }
}
