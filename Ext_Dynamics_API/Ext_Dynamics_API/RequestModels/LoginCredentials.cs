using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.RequestModels
{
    [NotMapped]
    public class LoginCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
