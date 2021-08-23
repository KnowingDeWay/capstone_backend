using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class SubmissionComment
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("author_id")]
        public int AuthorId { get; set; }

        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("edited_at")]
        public DateTime? EditedAt { get; set; }

        [JsonProperty("media_comment")]
        public MediaComment MediaComment { get; set; }
    }
}
