using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class AssignmentDate
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("base")]
        public bool Base { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("due_at")]
        public DateTime? DueAt { get; set; }

        [JsonProperty("unlock_at")]
        public DateTime? UnlockAt { get; set; }

        [JsonProperty("lock_at")]
        public DateTime? LockAt { get; set; }
    }
}
