using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class Grade
    {
        public string HtmlUrl { get; set; }
        public string CurrentGrade { get; set; }
        public string FinalGrade { get; set; }
        public string CurrentScore { get; set; }
        public string FinalScore { get; set; }
        public int CurrentPoints { get; set; }
        public string UnpostedCurrentGrade { get; set; }
        public string UnpostedFinalGrade { get; set; }
        public string UnpostedCurrentScore { get; set; }
        public string UnpostedFinalScore { get; set; }
        public int? UnpostedCurrentPoints { get; set; }
    }
}
