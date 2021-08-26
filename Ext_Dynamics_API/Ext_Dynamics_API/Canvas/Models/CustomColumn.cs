using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class CustomColumn
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("teacher_notes")]
        public bool TeacherNotes { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("read_only")]
        public bool ReadOnly { get; set; }
    }
}
