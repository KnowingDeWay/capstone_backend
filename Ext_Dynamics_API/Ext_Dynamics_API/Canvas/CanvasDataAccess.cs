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

        /// <summary>
        /// Gets all the courses that the user is an instructor in
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <returns>List&lt;Course>: The courses for which the user is an instructor in</returns>
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

        /// <summary>
        /// Gets all the assignments for a specified course
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <returns>List&lt;Assignment>: The assignment details for each assignment in the course</returns>
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

        /// <summary>
        /// Gets all the users in a course that are of a speific user type (e.g. Student, Teacher etc)
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="enrollmentType">The type of users to search for</param>
        /// <returns>List&lt;User>: The information of the Canvas users</returns>
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

        /// <summary>
        /// Gets a detailed breakdown of all the assignments and key statistics that describe the performance of a student,
        /// including assignment scores
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="studentId">The id of the student in Canvas</param>
        /// <returns>List&lt;UserCourseLevelAnalysis>: List of each statistical breakdown for each assignment</returns>
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

        /// <summary>
        /// Gets all the custom columns for a Canvas Gradebook for the specifed course
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <returns>List&lt;CustomColumn>: List of custom columns relating to the course Gradebook for Canvas</returns>
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

        /// <summary>
        /// Gets all the row data for a particular custom column
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="colId">The id of the custom column</param>
        /// <returns>List&lt;ColumnDatum>: The row data for the specified custom column</returns>
        public List<ColumnDatum> GetCustomColumnEntries(string accessToken, int courseId, int colId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/custom_gradebook_columns/{colId}/data";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var response = (HttpWebResponse)request.GetResponse();
            var receptionStream = response.GetResponseStream();
            var streamReader = new StreamReader(receptionStream, Encoding.UTF8);
            var resBody = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<List<ColumnDatum>>(resBody);
            return data;
        }

        /// <summary>
        /// Bulk updates the grades for an assignment column
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="assignmentId">The id of the assignment</param>
        /// <param name="entries">The list of entries detailing the new grade of each student</param>
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

        /// <summary>
        /// Sets all the entries for a custom column on Canvas
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="updateRequest">A request that contains the rows to update in the custom column</param>
        public void SetCustomColumnEntries(string accessToken, int courseId, CustomColumnsUpdateRequest updateRequest)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/custom_gradebook_column_data";
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

        /// <summary>
        /// Pushes and creates a new Custom Column on Canvas
        /// </summary>
        /// <param name="accessToken">The API key to use to access Canvas</param>
        /// <param name="request">The request containing all the information in regards to the structure of the column</param>
        /// <param name="courseId">The id of the course</param>
        /// <returns>CustomColumn: The column that was added to Canvas</returns>
        public CustomColumn AddNewCustomColumn(string accessToken, CustomColumnCreationRequest request, int courseId)
        {
            string requestUrl = $"/api/v1/courses/{courseId}/custom_gradebook_columns";
            var webRequest = new HttpRequestMessage(new HttpMethod("POST"), requestUrl);
            webRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
            var formContent = new MultipartFormDataContent
            {
                { new StringContent(request.Title), "column[title]" },
                { new StringContent($"{request.Position}"), "column[position]" },
                { new StringContent($"{request.Hidden}"), "column[hidden]" },
                { new StringContent($"{request.ReadOnly}"), "column[read_only]" }
            };
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

        /// <summary>
        /// Updates the custom column on Canvas
        /// </summary>
        /// <param name="accessToken">The API Key to use with Canvas APIs</param>
        /// <param name="updateRequest">The request containing the updated details of the column</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="colId">The id of the column to update</param>
        /// <returns>CustomColumn: The details of the updated column on Canvas</returns>
        public CustomColumn EditCustomColumn(string accessToken, CustomColumnCreationRequest updateRequest, int courseId, int colId)
        {
            string requestUrl = $"/api/v1/courses/{courseId}/custom_gradebook_columns/{colId}";
            var webRequest = new HttpRequestMessage(new HttpMethod("PUT"), requestUrl);
            webRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
            var formContent = new MultipartFormDataContent
            {
                { new StringContent(updateRequest.Title), "column[title]" },
                { new StringContent($"{updateRequest.Position}"), "column[position]" },
                { new StringContent($"{updateRequest.Hidden}"), "column[hidden]" },
                { new StringContent($"{updateRequest.ReadOnly}"), "column[read_only]" }
            };
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

        /// <summary>
        /// Deletes a custom column from Canvas
        /// </summary>
        /// <param name="accessToken">The API Key to access the Canvas API</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="colId">The id of the custom column on Canvas</param>
        /// <returns>CustomColumn: Details of the column that was deleted</returns>
        public CustomColumn DeleteCustomColumn(string accessToken, int courseId, int colId)
        {
            string requestUrl = $"{_config.canvasBaseUrl}/courses/{courseId}/custom_gradebook_columns/{colId}";
            var request = WebRequest.Create(requestUrl);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Method = "DELETE";
            var response = (HttpWebResponse)request.GetResponse();
            CustomColumn column;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string columnJson = streamReader.ReadToEnd();
                column = JsonConvert.DeserializeObject<CustomColumn>(columnJson);
            }
            return column;
        }

        /// <summary>
        /// Disposes this class and the resources it utilizes
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this); // Prevents derived types from having to call this method again
            ((IDisposable)_httpClient).Dispose();
        }
    }
}
