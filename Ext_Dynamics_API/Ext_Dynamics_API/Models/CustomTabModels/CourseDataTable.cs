using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Canvas.Enums.Params;
using Ext_Dynamics_API.Canvas.Models;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.ResourceManagement.CustomDataColumnManagement;
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
        public List<Submission> Submissions { get; set; } // List of Assignment Submissions

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
            table.AssignmentGradeColumns.AddRange(assignmentCols);
            PopulateAssignmentRows(ref table, accessToken, courseId);

            // Load custom data columns into table with row data
            GetCustomDataColumns(ref table, accessToken, courseId, dbContext);

            return table;
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
                var col = new DataColumn
                {
                    Name = assignment.Name,
                    RelatedDataId = assignment.Id,
                    ColumnType = ColumnType.Assignment_Score,
                    DataType = ColumnDataType.Number,
                    ColMaxValue = assignment.PointsPossible,
                    ColMinValue = 0,
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
                            Value = cdata.Submission.Score,
                            DataColumn = col,
                            AssociatedUser = student,
                            ValueChanged = false
                        };
                        col.Rows.Add(row);
                    }
                }
            }
        }

        private static void GetCustomDataColumns(ref CourseDataTable table, string accessToken, int courseId, 
            ExtensibleDbContext dbCtx)
        {
            var customCols = new List<DataColumn>();
            var config = SystemConfig.LoadConfig();
            var colManager = new CustomColumnManager(dbCtx, table.Students);
            table.CustomDataColumns = colManager.GetCustomDataColumns(accessToken, courseId);
        }
    }
}
