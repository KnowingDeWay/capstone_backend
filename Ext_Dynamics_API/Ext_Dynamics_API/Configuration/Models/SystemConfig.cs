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

        [JsonProperty(PropertyName = "test_db_conn_str")]
        public readonly string testDbConnString;

        [JsonProperty(PropertyName = "canvas_base_url")]
        public readonly string canvasBaseUrl;

        [JsonProperty(PropertyName = "canvas_namespace")]
        public readonly string canvasNamespace;

        /// <summary>
        /// Loads the configuration settings from the "main_config.json" file in the build directory under Configuration/Models
        /// </summary>
        /// <returns>SystemConfig: The runtime System Configuration of the application</returns>
        public static SystemConfig LoadConfig()
        {
            var currPath = Environment.CurrentDirectory;
            return JsonConvert.DeserializeObject<SystemConfig>(File.ReadAllText($"{currPath}/Configuration/main_config.json"));
        }
    }
}
