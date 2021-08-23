using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class LockInfo
    {
        [JsonProperty("asset_string")]
        public string AssetString { get; set; }

        [JsonProperty("unlock_at")]
        public DateTime UnlockAt { get; set; }

        [JsonProperty("lock_at")]
        public DateTime LockAt { get; set; }

        [JsonProperty("context_module")]
        public string ContextModule { get; set; }

        [JsonProperty("manually_locked")]
        public bool ManuallyLocked { get; set; }
    }
}
