using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class NumericDataColumn : DataColumn
    {
        // The row data is not meant to be stored in our database, this is better off retreived from Canvas
        public List<NumericDataRow> Rows { get; set; }

        public double this[string name]
        {
            get
            {
                var row = Rows.Where(x => x.AssociatedUser.Name.Equals(name)).FirstOrDefault();
                if(row != null)
                {
                    return row.Value;
                }
                return double.NaN;
            }
            set
            {
                var row = Rows.Where(x => x.AssociatedUser.Name.Equals(name)).FirstOrDefault();
                if (row != null)
                {
                    row.Value = value;
                }
            }
        }

        public double this[int id]
        {
            get
            {
                var row = Rows.Where(x => x.AssociatedUser.Id == id).FirstOrDefault();
                if (row != null)
                {
                    return row.Value;
                }
                return double.NaN;
            }
            set
            {
                var row = Rows.Where(x => x.AssociatedUser.Id == id).FirstOrDefault();
                if (row != null)
                {
                    row.Value = value;
                }
            }
        }
    }
}
