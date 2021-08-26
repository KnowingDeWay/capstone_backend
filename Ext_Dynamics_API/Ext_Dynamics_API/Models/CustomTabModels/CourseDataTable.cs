using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Canvas.Enums.Params;
using Ext_Dynamics_API.Canvas.Models;
using Ext_Dynamics_API.Configuration.Models;
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

        public static CourseDataTable LoadDataTable(int courseId, string accessToken)
        {
            var table = new CourseDataTable();
            table.CourseId = courseId;
            var students = GetStudents(courseId, accessToken);
            table.Students = students;
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

        private static List<DataColumn> GetAssignmentColumns()
        {
            var assignmentCols = new List<DataColumn>();
            return assignmentCols;
        }
    }
}
