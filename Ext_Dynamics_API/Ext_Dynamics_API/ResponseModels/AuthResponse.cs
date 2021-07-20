using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ext_Dynamics_API.ResponseModels
{
    [NotMapped]
    public class AuthResponse
    {
        public string ResponseMessage { get; set; }
        public string ResponseToken { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
