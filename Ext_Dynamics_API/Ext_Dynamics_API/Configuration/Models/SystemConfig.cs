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

        [JsonProperty(PropertyName = "token_security_key")]
        public readonly string tokenSecKey;

        [JsonProperty(PropertyName = "token_issuer")]
        public readonly string tokenIssuer;

        [JsonProperty(PropertyName = "token_audience")]
        public readonly string tokenAudience;

        [JsonProperty(PropertyName = "token_auth_header")]
        public readonly string authHeader;

        public static SystemConfig LoadConfig()
        {
            var currPath = Environment.CurrentDirectory;
            return JsonConvert.DeserializeObject<SystemConfig>(File.ReadAllText($"{currPath}/Configuration/main_config.json"));
        }
    }
}
