using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduleGeneratorProject.App_Code
{
    public class Schedule
    {
        public Course[] Courses { get; set; }
        public Schedule[] PossibleScheds { get; set; }

        public void GenerateSched()
        {
            throw new NotImplementedException();
        }

        public bool IsConflict()
        {
            throw new NotImplementedException();
        }
    }
}