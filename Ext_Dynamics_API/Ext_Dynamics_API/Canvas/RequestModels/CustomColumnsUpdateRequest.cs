using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.RequestModels
{
    public class CustomColumnsUpdateRequest
    {
        [JsonProperty("column_data")]
        public List<CustomColumnDataEntry> ColumnData { get; set; }
    }
}
