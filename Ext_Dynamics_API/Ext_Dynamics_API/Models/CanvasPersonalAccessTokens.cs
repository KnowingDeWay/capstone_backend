using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models
{
    public class CanvasPersonalAccessTokens
    {
        [Key]
        public int PatId { get; set; }
        public string TokenName { get; set; }
        
        // Encoded State of PAT (Personal Access Token)
        public string AccessToken { get; set; }
        public int AppUserId { get; set; }
    }
}
