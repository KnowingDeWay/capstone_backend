using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class GradingPeriod
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("close_date")]
        public DateTime CloseDate { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }
    }
}
