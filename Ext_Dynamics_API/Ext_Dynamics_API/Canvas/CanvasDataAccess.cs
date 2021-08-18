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
    }
}
