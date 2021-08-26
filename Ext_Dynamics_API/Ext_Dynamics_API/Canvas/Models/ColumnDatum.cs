using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class ColumnDatum
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }
}
