using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class StringDataColumn : CourseDataColumn
    {
        // The row data is not meant to be stored in our database, this is better off retreived from Canvas
        public List<StringDataRow> Rows { get; set; }

        public string this[string name]
        {
            get
            {
                var row = Rows.Where(x => x.AssociatedUser.Name.Equals(name)).FirstOrDefault();
                if (row != null)
                {
                    return row.Value;
                }
                return null;
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

        public string this[int id]
        {
            get
            {
                var row = Rows.Where(x => x.AssociatedUser.Id == id).FirstOrDefault();
                if (row != null)
                {
                    return row.Value;
                }
                return null;
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
