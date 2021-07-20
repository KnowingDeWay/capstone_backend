using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace Ext_Dynamics_API.Models
{
    public class ApplicationUserAccount
    {
        [Key]
        public int AppUserId { get; set; }
        public string AppUserName { get; set; }
        public string UserPassword { get; set; }

        // Access tokens to be used with Canvas
        public List<CanvasPersonalAccessTokens> AccessTokens { get; set; }
    }
}
