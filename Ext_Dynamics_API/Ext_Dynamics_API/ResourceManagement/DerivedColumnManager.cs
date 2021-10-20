using Ext_Dynamics_API.Models.CustomTabModels;
using System.Collections.Generic;
using NCalc2;

namespace Ext_Dynamics_API.ResourceManagement
{
    public class DerivedColumnManager
    {
        private readonly CourseDataTable _courseTable;

        public DerivedColumnManager(CourseDataTable table)
        {
            _courseTable = table;
        }

        /// <summary>
        /// Loads row data into a derived data column
        /// </summary>
        /// <param name="column">The dervied data column to load data into</param>
        public void LoadDerivedDataColumn(ref NumericDataColumn column)
        {
            var tableColumns = new List<NumericDataColumn>();
            foreach (var col in _courseTable.AssignmentGradeColumns)
            {
                tableColumns.Add((NumericDataColumn)col);
            }
            foreach (var col in _courseTable.CustomDataColumns)
            {
                // Keep in mind that derived columns cannot derive data from other derived columns
                if (col.DataType == ColumnDataType.Number && !col.Name.Equals(column) && col.ColumnType != ColumnType.Derived_Data)
                {
                    tableColumns.Add((NumericDataColumn)col);
                }
            }
            foreach (var student in _courseTable.Students)
            {
                var row = new NumericDataRow
                {
                    ColumnId = column.ColumnId,
                    ValueChanged = false,
                    AssociatedUser = student,
                };
                var expression = new Expression(column.CalcRule);
                foreach (var col in tableColumns)
                {
                    expression.Parameters[col.Name] = ((NumericDataColumn)_courseTable[col.Name])[student.Id];
                }
                row.Value = (double)expression.Evaluate();
                column.Rows.Add(row);
            }
        }
    }
}
