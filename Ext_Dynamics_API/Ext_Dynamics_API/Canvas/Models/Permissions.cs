using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class Permissions
    {
        [JsonProperty("create_discussion_topic")]
        public bool CreateDiscussionTopic { get; set; }

        [JsonProperty("create_announcement")]
        public bool CreateAnnouncement { get; set; }

        [JsonProperty("attach")]
        public bool Attach { get; set; }
    }
}
