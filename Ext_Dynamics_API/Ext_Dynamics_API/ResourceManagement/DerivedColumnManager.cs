using Ext_Dynamics_API.Models.CustomTabModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Expressions;

namespace Ext_Dynamics_API.ResourceManagement
{
    public class DerivedColumnManager
    {
        private readonly CourseDataTable _courseTable;

        public DerivedColumnManager(CourseDataTable table)
        {
            _courseTable = table;
        }

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
                var colNameDictionary = new Dictionary<string, double>();
                foreach(var col in tableColumns)
                {
                    colNameDictionary.Add(col.Name, ((NumericDataColumn)_courseTable[col.Name])[student.Id]);
                }
                row.Value = Eval.Execute<double>(column.CalcRule, colNameDictionary);
                column.Rows.Add(row);
            }
        }
    }
}
