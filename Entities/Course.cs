using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace ScheduleGeneratorProject.Entities
{
    public class Course
    {
        public List<Section> lectures;
        public List<Section> labs;
        public List<List<Section>> iterations;
        public string dept;
        public int number;
        public int creditHours;

        public Course(MySqlDataReader dReader)
        {
            lectures = new List<Section>();
            labs = new List<Section>();
            iterations = new List<List<Section>>();

            dReader.Read();
            dept = dReader.GetValue(0).ToString();
            number = (int)dReader.GetValue(1);
            creditHours = (int)dReader.GetValue(4);

            do
            {
                if (dReader[6].ToString() == "LEC")
                {
                    if (!CheckSections(lectures, dReader))
                    {
                        lectures.Add(new Section(dReader));
                    }

                }
                else if (dReader[6].ToString() != "DSO")
                {
                    if (!CheckSections(labs, dReader))
                    {
                        labs.Add(new Section(dReader));
                    }
                }
            } while (dReader.Read());
        }

        private bool CheckSections(List<Section> listToCheck, MySqlDataReader dReader)
        {
            foreach(Section item in listToCheck)
            {                  
                if (item.days == dReader[10].ToString() && item.startTime.ToString("HH:mm:ss") == dReader[8].ToString())
                {
                    item.lineNumbers.Add((int)dReader.GetValue(3));
                    item.lecturer.Add(dReader[7].ToString());
                    item.room.Add(dReader[12].ToString());
                    return true;
                }
            }
            return false;
        }

        public void IterateSections(bool includeOnlineAppt)
        {
            if (labs.Count == 0)
            {
                foreach (Section lecture in lectures)
                {
                    if (includeOnlineAppt)
                        iterations.Add(new List<Section>() { lecture });
                    else
                    {
                        if(lecture.days != "NULL")
                            iterations.Add(new List<Section>() { lecture });
                    }
                }
            }
            else if (lectures.Count == 0)
            {
                foreach (Section lab in labs)
                {
                    if (includeOnlineAppt)
                        iterations.Add(new List<Section>() { lab });
                    else
                    {
                        if (lab.days != "NULL")
                            iterations.Add(new List<Section>() { lab });
                    }
                }
            }
            else
            {
                //List<List<Section>> temp = new List<List<Section>>();
                //temp.Add(lectures);
                //temp.Add(labs);
                //List<Section> combo = new List<Section>();
                //rec_IterateSections(temp, 0, combo);

                foreach (Section lecture in lectures)
                {
                    foreach (Section lab in labs)
                    {
                        if (!IsConflict(lecture, lab, includeOnlineAppt))
                        {
                            iterations.Add(new List<Section>() { lecture, lab });
                        }
                    }
                }
            }
        }

        public bool IsConflict(Section lecture, Section lab, bool includeOnlineAppt)
        {
            if (lecture.days == "NULL" || lab.days == "NULL")
            {
                if (includeOnlineAppt)
                    return false;
                // if they don't want online classes, NULL for days is treated like a conflict
                else
                    return true;
            }
            else
            {
                if (lecture.days.Intersect(lab.days).Count() > 0)
                {
                    if (lecture.startTime >= lab.startTime && lecture.startTime <= lab.endTime)
                    {
                        return true;
                    }
                    else if (lecture.endTime >= lab.startTime && lecture.endTime <= lab.endTime)
                    {
                        return true;
                    }
                    else if (lecture.startTime <= lab.startTime && lecture.endTime >= lab.endTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        //private void rec_IterateSections(List<List<Section>> temp, int k, List<Section> combo)
        //{
        //    if (k >= 2)
        //    {
        //        List<Section> listCpy = new List<Section>();
        //        listCpy.Add(combo[0]);
        //        listCpy.Add(combo[1]);
        //        iterations.Add(listCpy);
        //        return;
        //    }

        //    foreach (Section currSection in temp[k])
        //    {
        //        combo.Add(currSection);
        //        rec_IterateSections(temp, k + 1, combo);
        //        combo.RemoveAt(combo.Count - 1);
        //    }
        //}

    }

    
}