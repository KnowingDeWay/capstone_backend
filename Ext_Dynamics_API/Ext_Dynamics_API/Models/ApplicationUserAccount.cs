using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Ext_Dynamics_API.Enums;

namespace Ext_Dynamics_API.Models
{
    public class ApplicationUserAccount : EntityBase
    {
        public string AppUserName { get; set; }
        public string UserPassword { get; set; }
        public UserType UserType { get; set; }

        // Access tokens to be used with Canvas
        public List<CanvasPersonalAccessTokens> AccessTokens { get; set; }
    }
}
