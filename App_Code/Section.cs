using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduleGeneratorProject.App_Code
{
    public class Section
    {
        public int[] LineNumbers { get; set; }
        public string SectionType { get; set; }
        public string Days { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Room { get; set; }
        public string Lecturer { get; set; }
    }
}