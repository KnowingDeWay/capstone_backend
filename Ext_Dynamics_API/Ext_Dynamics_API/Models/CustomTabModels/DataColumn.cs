using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class DataColumn
    {
        // X-axis label value of table
        public string Name { get; set; }
        public ColumnDataType DataType { get; set; }
        public ColumnType ColumnType { get; set; }
        public string CalcRule { get; set; }
        public List<DataRow> Rows { get; set; }
        public int? RelatedDataId { get; set; } // The id of the assignment on Canvas this column might be reffering to
        public int ColMaxValue { get; set; }
        public int ColMinValue { get; set; }
    }

    public enum ColumnDataType
    {
        // Int, Double, String, bool
        Number, Decimal, String, Boolean
    }

    public enum ColumnType
    {
        /*
         * Assignment Score: Assignment scores that Canvas uses to populate the Instructor's gradebook by default
         * Custom Canvas Column: User added columns added to the Course gradebook to hold additional data such as totals for example
         * File Import: A column added to the gradebook where the data is based off a file (a csv for example)
         * Derived Data: A column whose data is depedent on data within other columns. For example, a derived data column might
         * be a column where all the row values are equal to the sum of all the users assignments. Dervied Data columns are
         * automatically calculated by the system on the successful retreival of other data columns unlike all other column types.
         * Key Column: Column that is not saved but is rather used to represent the label values on the y-axis of the table.
         * For example, in the Canvas gradebook, the "key column" would be the student names.
         */
        Assignment_Score, Custom_Canvas_Column, File_Import, Derived_Data, Key_Column
    }
}
