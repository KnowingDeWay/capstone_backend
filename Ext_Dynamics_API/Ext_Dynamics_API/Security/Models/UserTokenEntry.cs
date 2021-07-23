using Ext_Dynamics_API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ext_Dynamics_API.Security.Models
{
    public class UserTokenEntry : EntityBase
    {
        public string EncodedToken { get; set; }
        public int AppUserId { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("AppUserId")]
        public ApplicationUserAccount UserAccount { get; set; }
    }
}
