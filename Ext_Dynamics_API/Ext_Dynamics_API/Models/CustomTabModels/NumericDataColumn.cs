﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class NumericDataColumn : DataColumn
    {
        // The row data is not meant to be stored in our database, this is better off retreived from Canvas
        public List<NumericDataRow> Rows { get; set; }
    }
}
