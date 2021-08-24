using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class UserDisplay
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("avatar_image_url")]
        public string AvatarImageUrl { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
    }
}
