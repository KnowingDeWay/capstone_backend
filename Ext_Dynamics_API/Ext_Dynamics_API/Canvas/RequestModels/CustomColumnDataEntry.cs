using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.RequestModels
{
    public class CustomColumnDataEntry
    {
        [JsonProperty("column_id")]
        public int ColumnId { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
