using Ext_Dynamics_API.Models.CustomTabModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models
{
    public class DataColumnEntry : EntityBase
    {
        // X-axis label value of table
        public string Name { get; set; }
        public ColumnDataType DataType { get; set; }
        public ColumnType ColumnType { get; set; }
        public string? CalcRule { get; set; }

        // The id of the assignment or the id of the custom column on Canvas this column might be reffering to
        // This is also an alternate key
        public int RelatedDataId { get; set; }
        public double ColMaxValue { get; set; } // Max permssible value that can be entered for numerical columns
        public double ColMinValue { get; set; } // Min permssible value that can be entered for numerical columns
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUserAccount UserAccount { get; set; }
        public int CourseId { get; set; }
    }
}
