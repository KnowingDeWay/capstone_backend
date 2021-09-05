using Ext_Dynamics_API.Canvas.AnalysisModels;
using Ext_Dynamics_API.Canvas.Enums;
using Ext_Dynamics_API.Canvas.Enums.Params;
using Ext_Dynamics_API.Canvas.Models;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas
{
    public class CanvasDataAccess
    {
        private readonly SystemConfig _config;

        public CanvasDataAccess(SystemConfig config)
        {
            _config = config;
        }

        public List<Course> GetInstructorCourses(string accessToken)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses?enrollment_type=teacher";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var courses = JsonConvert.DeserializeObject<List<Course>>(resBody);
            return courses;
        }

        public List<Assignment> GetCourseAssignments(string accessToken, int courseId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/assignments";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var assignments = JsonConvert.DeserializeObject<List<Assignment>>(resBody);
            return assignments;
        }

        public List<User> GetUsersInCourse(string accessToken, int courseId, EnrollmentParamType enrollmentType)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/users?enrollment_type={enrollmentType}";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var users = JsonConvert.DeserializeObject<List<User>>(resBody);
            return users;
        }

        public List<UserCourseLevelAnalysis> GetAnalysisData(string accessToken, int courseId, int studentId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/analytics/users/{studentId}/assignments";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<List<UserCourseLevelAnalysis>>(resBody);
            return data;
        }

        public List<CustomColumn> GetCustomColumns(string accessToken, int courseId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/custom_gradebook_columns";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<List<CustomColumn>>(resBody);
            return data;
        }

        public List<ColumnDatum> GetCustomColumnEntries(string accessToken, int courseId, int colId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/api/v1/courses/{courseId}/custom_gradebook_columns/{colId}/data";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<List<ColumnDatum>>(resBody);
            return data;
        }
    }
}
