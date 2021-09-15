using Ext_Dynamics_API.Canvas.AnalysisModels;
using Ext_Dynamics_API.Canvas.DataAccessModels;
using Ext_Dynamics_API.Canvas.Enums;
using Ext_Dynamics_API.Canvas.Enums.Params;
using Ext_Dynamics_API.Canvas.Models;
using Ext_Dynamics_API.Canvas.RequestModels;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.Enums;
using Ext_Dynamics_API.Models.CustomTabModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas
{
    public class CanvasDataAccess : IDisposable
    {
        private readonly SystemConfig _config;
        private readonly HttpClient _httpClient;

        public CanvasDataAccess(SystemConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config.canvasBaseUrl);
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

        public void SetAssignmentColumnEntries(string accessToken, int courseId, int assignmentId, 
            List<AssignmentGradeChangeEntry> entries)
        {
            string requestUrl = $"/api/v1/courses/{courseId}/assignments/{assignmentId}/submissions/update_grades";
            var request = new HttpRequestMessage(new HttpMethod("POST"), requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var formContent = new MultipartFormDataContent();
            foreach(var entry in entries)
            {
                formContent.Add(new StringContent($"{entry.NewGrade}"), $"grade_data[{entry.StudentId}][posted_grade]");
            }
            request.Content = formContent;
            var response = _httpClient.Send(request);

            // Clean up
            request.Dispose();
            response.Dispose();
        }

        public void SetCustomColumnEntries(string accessToken, int courseId, CustomColumnsUpdateRequest updateRequest)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/api/v1/courses/{courseId}/custom_gradebook_column_data";
            var request = WebRequest.CreateHttp(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.ContentType = "application/json";
            request.Method = "PUT";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string jsonContent = JsonConvert.SerializeObject(updateRequest);
                streamWriter.Write(jsonContent);
            }
            request.GetResponse();
        }

        public CustomColumn AddNewCustomColumn(string accessToken, CustomColumnCreationRequest request, int courseId)
        {
            string requestUrl = $"/api/v1/courses/{courseId}/custom_gradebook_columns";
            var webRequest = new HttpRequestMessage(new HttpMethod("POST"), requestUrl);
            webRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(request.Title), "column[title]");
            formContent.Add(new StringContent($"{request.Position}"), "column[position]");
            formContent.Add(new StringContent($"{request.Hidden}"), "column[hidden]");
            formContent.Add(new StringContent($"{request.TeacherNotes}"), "column[teacher_notes]");
            formContent.Add(new StringContent($"{request.ReadOnly}"), "column[read_only]");
            webRequest.Content = formContent;
            var response = _httpClient.Send(webRequest);
            CustomColumn column;
            using (var streamReader = new StreamReader(response.Content.ReadAsStream()))
            {
                string columnJson = streamReader.ReadToEnd();
                column = JsonConvert.DeserializeObject<CustomColumn>(columnJson);
            }

            // Clean up
            webRequest.Dispose();
            response.Dispose();

            return column;
        }

        public void DeleteCustomColumn(string accessToken, int courseId, int colId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/api/v1/courses/{courseId}/custom_gradebook_columns/{colId}";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Method = "DELETE";
            var response = (HttpWebResponse)request.GetResponse();
        }

        public void Dispose()
        {
            ((IDisposable)_httpClient).Dispose();
        }
    }
}
