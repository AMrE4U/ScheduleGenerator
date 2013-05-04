using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduleGeneratorProject.Entities
{
    public class Schedules
    {
        // for use in weighting for small and large gaps
        public class dayinfo
        {
            public double idealspan;
            public double actualspan;
            public DateTime minstart;
            public DateTime maxend;
            public double diff;
            public int numClasses;

            public dayinfo()
            {
                minstart = Convert.ToDateTime("11:00:00 PM");
                maxend = Convert.ToDateTime("6:00:00 AM");
                idealspan = 0;
                actualspan = 0;
                diff = 0;
                numClasses = 0;
            }
        }

        static public Dictionary<char, int> daysToIndices = new Dictionary<char, int>()
        {
                {'M', 0}, {'T', 1}, {'W', 2}, {'R', 3}, {'F', 4}, {'S', 5}, {'N', 6}
        };

        // schedule subclass primarily for adding weights to individual schedules within the schedules class
        public class Schedule
        {
            // total_weight for all of the preferences selected
            public double total_weight;

            // list of sections making up the courses in this schedule
            public List<Section> courses;

            public Schedule()
            {
                total_weight = 0;
                courses = new List<Section>();
            }

            // add all of the course durations together on given days, call it idealdur, and then find the diff between the earliest
            // start and latest end, call it actualdur, and for every hour the actualdur is greater than the idealdur, subtract 1 from 10
            public void calcSmGapWeight(double mult)
            {
                double avg = 0;
                string daysWithSections = "";

                // create an array that represents the days of the week, and fill those spots with dayinfo class objects
                dayinfo[] weekdays = new dayinfo[7];
                for (int i = 0; i < 7; i++)
                    weekdays[i] = new dayinfo();

                foreach (Section sect in this.courses)
                {
                    if (sect.days != "NULL")
                    {
                        foreach (char day in sect.days)
                        {
                            // keep track of which days need to be considered
                            daysWithSections = new string((daysWithSections + day.ToString()).Distinct().ToArray());

                            dayinfo info = weekdays[daysToIndices[day]];
                            if (sect.startTime < info.minstart)
                                info.minstart = sect.startTime;
                            if (sect.endTime > info.maxend)
                                info.maxend = sect.endTime;
                            info.idealspan += (sect.endTime - sect.startTime).TotalHours;
                            weekdays[daysToIndices[day]] = info;
                        }
                    }
                }
                // now calculate the actualspan and find the difference for each day needed
                foreach (char day in daysWithSections)
                {
                    dayinfo info = weekdays[daysToIndices[day]];
                    info.actualspan = (info.maxend - info.minstart).TotalHours;
                    info.diff = info.actualspan - info.idealspan;
                    avg += info.diff;
                }
                // take the average across all of the days with sections
                avg = avg / daysWithSections.Length;
                total_weight += (10 - avg) * mult;
            }

            // 17 hour span between 6am and 11pm. Idealspan = 17, actual span is still the span between the earliest start and
            // latest end times on that specific day. just one class during a day? should give 10.
            public void calcLgGapWeight(double mult)
            {
                double avg = 0;
                string daysWithSections = "";

                // create an array that represents the days of the week, and fill those spots with dayinfo class objects
                dayinfo[] weekdays = new dayinfo[7];
                for (int i = 0; i < 7; i++)
                    weekdays[i] = new dayinfo();

                foreach (Section sect in this.courses)
                {
                    if (sect.days != "NULL")
                    {
                        foreach (char day in sect.days)
                        {
                            // keep track of which days need to be considered
                            daysWithSections = new string((daysWithSections + day.ToString()).Distinct().ToArray());

                            dayinfo info = weekdays[daysToIndices[day]];
                            info.numClasses++;
                            if (sect.startTime < info.minstart)
                                info.minstart = sect.startTime;
                            if (sect.endTime > info.maxend)
                                info.maxend = sect.endTime;
                            info.idealspan += (sect.endTime - sect.startTime).TotalHours;
                            weekdays[daysToIndices[day]] = info;
                        }
                    }
                }
                // now calculate the actualspan and find the difference for each day needed
                foreach (char day in daysWithSections)
                {
                    dayinfo info = weekdays[daysToIndices[day]];
                    // if there is only one day, the gap is infinite, so give it a perfect weight
                    if (info.numClasses == 1)
                        avg += 10;
                    else
                    {
                        info.actualspan = (info.maxend - info.minstart).TotalHours;
                        // actual span is always >= idealspan. ideal must be at least 2, actual best case is 17.
                        // 17-2 = 15-5 = 10 which is the perfect scenario.
                        info.diff = (info.actualspan - info.idealspan) - 5;
                        avg += info.diff;
                    }
                }
                // take the average across all of the days with sections
                avg = avg / daysWithSections.Length;
                total_weight += avg * mult;
            }

            // take each section, look at the days that section is taught on, and concatenate the distinct elements to a 
            // days string. Once done, compare the amount of days off to the days of the week
            public void calcMostDaysOffWeight(double mult)
            {
                string days = "";
                double avg = 0;
                foreach (Section sect in this.courses)
                {
                    // if there is a day specified with this section
                    if (sect.days != "NULL")
                    {
                        days = new string((days += sect.days).Distinct().ToArray());
                    }
                }
                int numdaysoff = 7 - days.Length;
                // if numdaysoff = 6 => 10, 5 => 8, 4 => 6, 3 => 4, 2 => 2, 1 => 0, 0 => -2
                total_weight += ((numdaysoff*2) - 2)*mult;
            }

            // calculates a weight based on the entire schedule rather than just one section
            public void calcSchedWeight(List<string> pref, List<double> mult)
            {
                for (int i = 0; i < pref.Count; i++)
                {
                    if (pref[i] == "smGap")
                    {
                        calcSmGapWeight(mult[i]);
                    }
                    if (pref[i] == "lgGap")
                    {
                        calcLgGapWeight(mult[i]);
                    }
                    if (pref[i] == "mostdaysoff")
                    {
                        calcMostDaysOffWeight(mult[i]);
                    }
                }
            }
        }

        public List<Schedule> possibleScheds;
        private int ctr;
        public int schedWideWeighting;
        public List<string> schedWidePref;
        public List<double> schedWideMult;

        public Schedules()
        {
            possibleScheds = new List<Schedule>();
            ctr = 0;
            schedWideWeighting = 0;
        }

        public void GenerateSched(List<Course> courseList, int schedWideFlag, List<string> pref, List<double> mult, bool includeOnlineAppt)
        {
            List<List<List<Section>>> temp = new List<List<List<Section>>>();

            // does a weight need to be added that is schedule wide?
            this.schedWideWeighting = schedWideFlag;
            this.schedWidePref = pref;
            this.schedWideMult = mult;

            foreach (Course course in courseList)
            {
                course.IterateSections(includeOnlineAppt);
                temp.Add(course.iterations);
            }

            Schedule currSched = new Schedule();

            rec_GenerateSchedules(temp, courseList.Count(), 0, 0, currSched);

            // in case there are less than 10 elements after we add all of the schedules
            if (possibleScheds.Count < 20)
                possibleScheds = possibleScheds.OrderBy(Schedule => Schedule.total_weight).ToList(); 

        }


        
        private void addtoList(List<Schedule> possibleScheds, Schedule tobeInserted)
        {
            // walk through the list of possibleScheds and see if tobeInserted can fit
            int count = possibleScheds.Count;
            int i = 0;
            // we need to auto add the first 10, then sort the list
            if (count < 20)
            {
                possibleScheds.Add(tobeInserted);
                // when the 10th element is added, sort the 10 element list.
                if (count == 19)
                    this.possibleScheds = possibleScheds.OrderBy(Schedule => Schedule.total_weight).ToList(); 
                return;
            }

            double compWeight = tobeInserted.total_weight;
            // insertion sort
            while ((i < count) && (compWeight > possibleScheds[i].total_weight))
                i++;
            // if it broke out and i == 0, it doesnt belong on the list.
            if (i == 0)
                return;
            // if it broke the loop and 0 < i <= count, then tobeInserted is in the list.
            else
            {
                possibleScheds.RemoveAt(0);
                possibleScheds.Insert(i - 1, tobeInserted);
            }
        }

        private void rec_GenerateSchedules(List<List<List<Section>>> temp, int courseCount, int loopCount, int k, Schedule currSched)
        {
            if (loopCount >= courseCount && currSched.courses.Count() != 0)
            {
                Schedule listCpy = new Schedule();
                foreach (Section section in currSched.courses)
                {
                    listCpy.courses.Add(section);
                    listCpy.total_weight += section.weight;
                }

                // add an extra weight for the schedule as a whole
                if (this.schedWideWeighting == 1)
                    listCpy.calcSchedWeight(this.schedWidePref, this.schedWideMult);

                addtoList(possibleScheds, listCpy);
                //ctr++;
                //System.Diagnostics.Debug.WriteLine(ctr);
                return;
            }

            foreach (List<Section> currIteration in temp[k])
            {
                //test each section in the iteration for a conflict with the schedule.
                bool test = false;
                foreach (Section currSection in currIteration)
                {
                    if (IsConflict(currSection, currSched.courses))
                    {
                        test = true;
                    }
                }

                //if no conflict test=false then add the sections to the currSched
                if (!test)
                {
                    foreach (Section currSection in currIteration)
                    {
                        currSched.courses.Add(currSection);
                    }
                    rec_GenerateSchedules(temp, courseCount, loopCount + 1, k + 1, currSched);
                    for (int i = 0; i < currIteration.Count(); i++)
                    {
                        currSched.courses.RemoveAt(currSched.courses.Count() - 1);

                    }
                }
            }
        }

        public bool IsConflict(Section sectionToCheck, List<Section> currSched)
        {
            if (sectionToCheck.days == "NULL")
            {
                return false;
            }
            else
            {
                foreach (Section currSection in currSched)
                {
                    if (sectionToCheck.days.Intersect(currSection.days).Count() > 0)
                    {
                        if (sectionToCheck.startTime >= currSection.startTime && sectionToCheck.startTime <= currSection.endTime)
                        {
                            return true;
                        }
                        else if (sectionToCheck.endTime >= currSection.startTime && sectionToCheck.endTime <= currSection.endTime)
                        {
                            return true;
                        }
                        // if a section surrounds another section (lab from 7:30 to 11:20 as sectiontocheck and lec from 10 to 10:50 as currsection) 
                        else if (sectionToCheck.startTime <= currSection.startTime && sectionToCheck.endTime >= currSection.endTime)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }

}