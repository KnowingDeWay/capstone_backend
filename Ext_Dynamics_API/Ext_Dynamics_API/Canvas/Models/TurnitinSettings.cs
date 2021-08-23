using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class TurnitinSettings
    {
        [JsonProperty("originality_report_visibility")]
        public string OriginalityReportVisibility { get; set; }

        [JsonProperty("s_paper_check")]
        public bool SPaperCheck { get; set; }

        [JsonProperty("internet_check")]
        public bool InternetCheck { get; set; }

        [JsonProperty("journal_check")]
        public bool JournalCheck { get; set; }

        [JsonProperty("exclude_biblio")]
        public bool ExcludeBiblio { get; set; }

        [JsonProperty("exclude_quoted")]
        public bool ExcludeQuoted { get; set; }

        [JsonProperty("exclude_small_matches_type")]
        public string ExcludeSmallMatchesType { get; set; }

        [JsonProperty("exclude_small_matches_value")]
        public int ExcludeSmallMatchesValue { get; set; }
    }
}
