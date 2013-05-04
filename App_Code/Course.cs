using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduleGeneratorProject.App_Code
{
    public class Course
    {
        public Section[] Lectures { get; set; }
        public Section[] Labs { get; set; }
        public string Dept { get; set; }
        public int Number { get; set; }
        public int CreditHours { get; set; }
    }
}