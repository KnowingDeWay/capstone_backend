using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models
{
    [ValidateNever]
    public class CanvasPersonalAccessToken : EntityBase
    {
        public string TokenName { get; set; }
        
        // Encoded State of PAT (Personal Access Token)
        public string AccessToken { get; set; }
        public int AppUserId { get; set; }
        public bool TokenActive { get; set; }

        [ForeignKey("AppUserId")]
        public ApplicationUserAccount ApplicationUser { get; set; }
    }
}
