using Ext_Dynamics_API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResponseModels
{
    [NotMapped]
    public class UserProfile
    {
        public string AppUserName { get; set; }
        public UserType UserType { get; set; }
    }
}
