using System;
using System.Collections.Generic;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class CourseDataColumn
    {
        public Guid ColumnId { get; set; }

        // X-axis label value of table
        public string Name { get; set; }
        public ColumnDataType DataType { get; set; }
        public ColumnType ColumnType { get; set; }
        public string CalcRule { get; set; }
        public int RelatedDataId { get; set; } // The id of the assignment/custom tab on Canvas this column might be reffering to
        public double ColMaxValue { get; set; } // Max permssible value that can be entered for numerical columns
        public double ColMinValue { get; set; } // Min permssible value that can be entered for numerical columns

        /// <summary>
        /// Creates a typed column based off the existing column
        /// </summary>
        /// <returns>CourseDataColumn: Creates a typed column (numeric or string column)</returns>
        public CourseDataColumn ConvertToTypedColumn()
        {
            CourseDataColumn newCol;
            if(DataType == ColumnDataType.Number)
            {
                newCol = new NumericDataColumn()
                {
                    Name = Name,
                    DataType = DataType,
                    ColumnType = ColumnType,
                    CalcRule = CalcRule,
                    RelatedDataId = RelatedDataId,
                    ColMaxValue = ColMaxValue,
                    ColMinValue = ColMinValue,
                    Rows = new List<NumericDataRow>()
                };
            }
            else
            {
                newCol = new StringDataColumn()
                {
                    Name = Name,
                    DataType = DataType,
                    ColumnType = ColumnType,
                    CalcRule = CalcRule,
                    RelatedDataId = RelatedDataId,
                    ColMaxValue = ColMaxValue,
                    ColMinValue = ColMinValue,
                    Rows = new List<StringDataRow>()
                };
            }
            return newCol;
        }
    }

    public enum ColumnDataType
    {
        // Double, String
        Number, String
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
         */
        Assignment_Score, Custom_Canvas_Column, File_Import, Derived_Data
    }
}
