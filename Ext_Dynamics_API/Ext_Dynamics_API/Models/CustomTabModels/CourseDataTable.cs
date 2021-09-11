using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Canvas.DataAccessModels;
using Ext_Dynamics_API.Canvas.Enums.Params;
using Ext_Dynamics_API.Canvas.Models;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.ResourceManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models.CustomTabModels
{
    public class CourseDataTable
    {
        public List<DataColumn> AssignmentGradeColumns { get; set; }
        public List<DataColumn> CustomDataColumns { get; set; }
        public List<DataTableStudent> Students { get; set; } // Student names for example, y-axis values of the table
        public int CourseId { get; set; }

        private CourseDataTable()
        {
            
        }

        public static CourseDataTable LoadDataTable(int courseId, string accessToken, ExtensibleDbContext dbContext)
        {
            var table = new CourseDataTable
            {
                CourseId = courseId
            };

            // Load students
            var students = GetStudents(courseId, accessToken);
            table.Students = students;

            // Load assignment columns and data rows
            var assignmentCols = GetAssignmentColumns(courseId, accessToken);
            table.AssignmentGradeColumns = assignmentCols;
            PopulateAssignmentRows(ref table, accessToken, courseId);

            // Load custom data columns into table with row data
            GetCustomDataColumns(ref table, accessToken, courseId, dbContext);

            return table;
        }

        public static CourseDataTable LoadDataTableFromDynamicObject(dynamic table)
        {
            var newTable = new CourseDataTable
            {
                CourseId = table.courseId,
                AssignmentGradeColumns = new List<DataColumn>(),
                CustomDataColumns = new List<DataColumn>(),
                Students = new List<DataTableStudent>()
            };
            foreach(var col in table.assignmentGradeColumns)
            {
                var newCol = new NumericDataColumn
                {
                    Name = col.name,
                    DataType = col.dataType,
                    ColumnId = col.columnId,
                    ColumnType = col.columnType,
                    CalcRule = col.calcRule,
                    RelatedDataId = col.relatedDataId,
                    ColMaxValue = col.colMaxValue,
                    ColMinValue = col.colMinValue,
                    Rows = new List<NumericDataRow>()
                };
                foreach(var row in col.rows)
                {
                    var newRow = new NumericDataRow
                    {
                        ColumnId = row.columnId,
                        AssociatedUser = new DataTableStudent
                        {
                            Id = row.associatedUser.id,
                            Name = row.associatedUser.name
                        },
                        ValueChanged = row.valueChanged,
                        Value = row.value,
                        NewValue = row.newValue
                    };
                    newCol.Rows.Add(newRow);
                }
                newTable.AssignmentGradeColumns.Add(newCol);
            }
            foreach (var col in table.customDataColumns)
            {
                if(col.dataType == ColumnDataType.Number)
                {
                    var newCol = new NumericDataColumn
                    {
                        Name = col.name,
                        DataType = col.dataType,
                        ColumnId = col.columnId,
                        ColumnType = col.columnType,
                        CalcRule = col.calcRule,
                        RelatedDataId = col.relatedDataId,
                        ColMaxValue = col.colMaxValue,
                        ColMinValue = col.colMinValue,
                        Rows = new List<NumericDataRow>()
                    };

                    foreach (var row in col.rows)
                    {
                        var newRow = new NumericDataRow
                        {
                            ColumnId = row.columnId,
                            AssociatedUser = new DataTableStudent
                            {
                                Id = row.associatedUser.id,
                                Name = row.associatedUser.name
                            },
                            ValueChanged = row.valueChanged,
                            Value = row.value,
                            NewValue = row.newValue
                        };
                        newCol.Rows.Add(newRow);
                    }

                    newTable.AssignmentGradeColumns.Add(newCol);
                }
                else
                {
                    var newCol = new StringDataColumn
                    {
                        Name = col.name,
                        DataType = col.dataType,
                        ColumnId = col.columnId,
                        ColumnType = col.columnType,
                        CalcRule = col.calcRule,
                        RelatedDataId = col.relatedDataId,
                        ColMaxValue = col.colMaxValue,
                        ColMinValue = col.colMinValue,
                        Rows = new List<StringDataRow>()
                    };

                    foreach (var row in col.rows)
                    {
                        var newRow = new StringDataRow
                        {
                            ColumnId = row.columnId,
                            AssociatedUser = new DataTableStudent
                            {
                                Id = row.associatedUser.id,
                                Name = row.associatedUser.name
                            },
                            ValueChanged = row.valueChanged,
                            Value = row.value,
                            NewValue = row.newValue
                        };
                        newCol.Rows.Add(newRow);
                    }

                    newTable.AssignmentGradeColumns.Add(newCol);
                }
            }
            foreach(var student in table.students)
            {
                newTable.Students.Add(new DataTableStudent
                {
                    Id = student.id,
                    Name = student.name
                });
            }
            return newTable;
        }

        // Load students
        private static List<DataTableStudent> GetStudents(int courseId, string accessToken)
        {
            var config = SystemConfig.LoadConfig();
            var dataAccess = new CanvasDataAccess(config);
            var students = dataAccess.GetUsersInCourse(accessToken, courseId, EnrollmentParamType.student);
            var tableStudents = new List<DataTableStudent>();
            foreach(var student in students)
            {
                var tbStudent = new DataTableStudent
                {
                    Name = student.Name,
                    Id = student.Id
                };
                tableStudents.Add(tbStudent);
            }
            return tableStudents;
        }

        private static List<DataColumn> GetAssignmentColumns(int courseId, string accessToken)
        {
            var assignmentCols = new List<DataColumn>();
            var config = SystemConfig.LoadConfig();
            var dataAccess = new CanvasDataAccess(config);
            var assignments = dataAccess.GetCourseAssignments(accessToken, courseId);
            foreach(var assignment in assignments)
            {
                var col = new NumericDataColumn
                {
                    Name = assignment.Name,
                    RelatedDataId = assignment.Id,
                    ColumnId = Guid.NewGuid(),
                    ColumnType = ColumnType.Assignment_Score,
                    DataType = ColumnDataType.Number,
                    ColMaxValue = assignment.PointsPossible,
                    ColMinValue = 0,
                    Rows = new List<NumericDataRow>()
                };
                assignmentCols.Add(col);
            }
            return assignmentCols;
        }

        private static void PopulateAssignmentRows(ref CourseDataTable table, string accessToken, int courseId)
        {
            var config = SystemConfig.LoadConfig();
            var dataAccess = new CanvasDataAccess(config);
            foreach(var student in table.Students)
            {
                var data = dataAccess.GetAnalysisData(accessToken, courseId, student.Id);
                foreach(var col in table.AssignmentGradeColumns)
                {
                    var colData = data.Where(x => x.AssignmentId == col.RelatedDataId).ToList();
                    foreach(var cdata in colData)
                    {
                        var row = new NumericDataRow
                        {
                            Value = (cdata.Submission.Score != null) ? (double)cdata.Submission.Score : 0,
                            ColumnId = col.ColumnId,
                            AssociatedUser = student,
                            ValueChanged = false
                        };
                        ((NumericDataColumn)col).Rows.Add(row);
                    }
                }
            }
        }

        private static void GetCustomDataColumns(ref CourseDataTable table, string accessToken, int courseId, 
            ExtensibleDbContext dbCtx)
        {
            var customCols = new List<DataColumn>();
            var config = SystemConfig.LoadConfig();
            var colManager = new CourseDataTableManager(dbCtx, table.Students);
            table.CustomDataColumns = colManager.GetCustomDataColumns(accessToken, courseId);
        }
    }
}
