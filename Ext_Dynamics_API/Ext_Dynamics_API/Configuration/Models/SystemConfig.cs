using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Configuration.Models
{
    public class SystemConfig
    {
        [JsonProperty(PropertyName = "db_conn_str")]
        public readonly string dbConnString;
        public static SystemConfig LoadConfig()
        {
            var currPath = Environment.CurrentDirectory;
            return JsonConvert.DeserializeObject<SystemConfig>(File.ReadAllText($"{currPath}/Configuration/main_config.json"));
        }
    }
}
