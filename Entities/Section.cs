using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ScheduleGeneratorProject.Entities
{ 
    public class Section
    {
        public string dept;
        public int number;
        public List<int> lineNumbers;
        public string sectionType;
        public string days;
        public DateTime startTime;
        public DateTime endTime;
        public List<string> room;
        public List<string> lecturer;
        public double weight;

        public Section(IDataRecord dataRecord)
        {
            dept = dataRecord.GetValue(0).ToString();
            number = (int)dataRecord.GetValue(1);
            lineNumbers = new List<int>();
            lineNumbers.Add((int)dataRecord.GetValue(3));
            sectionType = dataRecord[6].ToString();
            days = dataRecord[10].ToString();
            startTime = Convert.ToDateTime(dataRecord[8].ToString());
            endTime = Convert.ToDateTime(dataRecord[9].ToString());
            room = new List<string>();
            room.Add(dataRecord[12].ToString());
            lecturer = new List<string>();
            lecturer.Add(dataRecord[7].ToString());
            weight = 0;
        }

        // if timePref is returned as a number (i.e. 17 for morning, 0 for evening) 
        // we can just do the operations without checking the input.
        public void calcWeightTimePref(string timePref, double mult)
        {
            DateTime sixam = Convert.ToDateTime("06:00:00 AM");
            TimeSpan diff = this.startTime - sixam;
            double timePastSix = diff.TotalHours;

            if(timePastSix >= 0)
            {
                // will gave a positive weight until 4pm then goes negative as it gets later
                if (timePref == "morn")
                    this.weight += (10 - timePastSix) * mult;
                // 10-4 wtih 10 and 4 worth 4, 11 and 3 worth 6, 12 and 2 worth 8, and 1 worth 10
                else if (timePref == "after")
                    // abs value func with opens down shifted up 10 right 7 with a slope of 2
                    this.weight += (-2*Math.Abs(timePastSix-7)+10)*mult;
                // positive weight until 4pm then negative as it gets earlier
                else
                    this.weight += (timePastSix - 7) * mult;
            }
            // if there is no time for this class, it fits your schedule perfectly no matter what
            else
                this.weight += 10*mult;

            int i = 0;
            
        }
        // dayPref should be in the form "MWF" or "TR"
        public void calcWeightDayPref(string dayPref, double mult)
        {
            // contains because it could be a "M" class and dayPref could be "MWF"
            if (dayPref.Contains(this.days) || this.days == "NULL")
                this.weight += 10 * mult;
        }

        // 
        public void calcWeightMiscPref(string miscPref, double mult)
        {

        }
    }
}