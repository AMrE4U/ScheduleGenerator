using MySql.Data.MySqlClient;
using ScheduleGeneratorProject.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ScheduleGeneratorProject
{
    public partial class results : System.Web.UI.Page
    {
        
        const int weight = 17;
        public string conStr = ConfigurationManager.ConnectionStrings["DB_mysql2"].ConnectionString;

        public Schedules schedToReturn = new Schedules();
        public List<Course> tempCourses = new List<Course>();
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                message.Text = (string)(Session["ErrorMessage"]);
                Session.Contents.Remove("ErrorMessage");
                //find control from previous page to get values from hidden field
                ContentPlaceHolder content = (ContentPlaceHolder)PreviousPage.Master.FindControl("MainContent");
                HiddenField saved = (HiddenField)content.FindControl("hiddenSavedSchedule");
                string saveStr = saved.Value;
                if (saveStr == "savedSchedule")
                {
                    retrieveSchedules();
                }
                else
                {
                HiddenField courses = (HiddenField)content.FindControl("hiddenCourseList");
                    HiddenField preferences = (HiddenField)content.FindControl("hiddenPreferences");
                    ContentPlaceHolder featuredContent = (ContentPlaceHolder)PreviousPage.Master.FindControl("FeaturedContent");
                    CheckBox online = (CheckBox)featuredContent.FindControl("onlineCB");

                string[] prefStr = preferences.Value.Split(',');

                string courseList = courses.Value;

                string dayPref = "";
                string timePref = ""; 
                string miscPref = "";
                int schedWideFlag = 0;
                List<string> schedWidePref = new List<string>();
                List<double> schedWideMult = new List<double>();
                double miscmult = 0;
                double daymult = 0;
                double timemult = 0;
                double mult = 1;
                bool includeOnlineAppt = online.Checked;

                // multindex will start at 1, each time this loop iterates it will be halved, until all of the contraints
                // have been accounted for.
                foreach (string pref in prefStr)
                {
                    if (pref == "morn" || pref == "after" || pref == "night")
                    {
                        timePref = pref;
                        timemult = mult;
                    }
                    else if (pref == "TR" || pref == "MWF")
                    {
                        dayPref = pref;
                        daymult = mult;
                    }
                    else
                    {
                        schedWidePref.Add(pref);
                        schedWideMult.Add(mult);
                        schedWideFlag = 1;
                    }
                    mult = mult / 2.0;
                }

                //check if courseList is empty, if so redirect
                if (courseList == "")
                {
                    //Session["ErrorMessage"] = textthing;
                    Session["ErrorMessage"] = "You must select at least one course.";
                    Response.Redirect("Default.aspx");
                }

                //split string result from hidden field and load into array
                string[] courseArray = courseList.Split(',');
                
                //call function to create courses from array of names
                CreateCourses(courseArray);

                // weight each section for each course
                calcSectionWeights(timePref, dayPref, miscPref, timemult, daymult, miscmult);

                // if there is a preference that requires the schedules as a whole to be weighted
              

                schedToReturn.GenerateSched(tempCourses, schedWideFlag, schedWidePref, schedWideMult, includeOnlineAppt);

                createJSON();
            }
        }
        }

        private void CreateCourses(string[] courseArray)
        {            
            foreach (string name in courseArray)
            {
                //split name into seperate dept and number fields
                string[] fields = name.Split(' ');

                //query database for courses data
                MySqlConnection con = new MySqlConnection(conStr);
                con.Open();
                string sqlQuery = "SELECT * FROM mcourses WHERE dept=@dept AND comnum=@comnum";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, con);
                cmd.Parameters.AddWithValue("@dept", fields[0]);
                cmd.Parameters.AddWithValue("@comnum", fields[1]);
                MySqlDataReader dReader = cmd.ExecuteReader();

                //create Course to load data into
                //schedToReturn.courses.Add(new Course(dReader));

                //add Course to temp list
                tempCourses.Add(new Course(dReader));
                dReader.Close();
                con.Close();
            }
        }

        // goes through each section of each course and calls the sections weighting function
        public void calcSectionWeights(string timePref, string dayPref, string miscPref, double timemult, double daymult, double miscmult)
        {
            // check which preferences are set in here to reduce amount of total checking since
            // we wont have to check for every section
            if (timemult != 0)
            {
                foreach (Course currCourse in tempCourses)
                {
                    foreach (Section sect in currCourse.lectures)
                    {
                        sect.calcWeightTimePref(timePref, timemult);
                    }
                    foreach (Section sect in currCourse.labs)
                    {
                        sect.calcWeightTimePref(timePref, timemult);
                    }
                }
            }
            if(daymult != 0)
            {
                foreach (Course currCourse in tempCourses)
                {
                    foreach (Section sect in currCourse.lectures)
                    {
                        sect.calcWeightDayPref(dayPref, daymult);
                    }
                    foreach (Section sect in currCourse.labs)
                    {
                        sect.calcWeightDayPref(dayPref, daymult);
                    }
                }
            }
            if(miscmult != 0)
            {
                foreach (Course currCourse in tempCourses)
                {
                    foreach (Section sect in currCourse.lectures)
                    {
                        sect.calcWeightMiscPref(miscPref, miscmult);
                    }
                    foreach (Section sect in currCourse.labs)
                    {
                        sect.calcWeightMiscPref(miscPref, miscmult);
                    }
                }
            }
        }

        public void createJSON()
        {
            DateTime today = DateTime.Today;
            int delta = DayOfWeek.Monday - today.DayOfWeek;
            DateTime monday = today.AddDays(delta);
            int count = 0;

            // since weights are sorted in increasing order, reverse the possibleScheds list to get max weight at the top
            schedToReturn.possibleScheds.Reverse();

            string JSONString = "[";
            foreach (Schedules.Schedule currSched in schedToReturn.possibleScheds)
            {
                DateTime schedDate = monday.AddDays(count);

                foreach (Section currClass in currSched.courses)
                {
                    int moveDay = 0;
                    // if there are no days, it needs to be "any-time"
                    if (currClass.days == "NULL")
                    {
                        //currClass.days = ""; // so that it doesn't parse "NULL" below
                        JSONString += "{";
                        JSONString += " title: '" + currClass.dept + " " + currClass.number.ToString() + " - " + currClass.sectionType + "',";
                        JSONString += " start: '" + schedDate.ToString("yyyy-MM-dd") + "',"; // documentation says it should span 7 days
                        JSONString += " end: '" + schedDate.AddDays(6).ToString("yyyy-MM-dd") + "',"; // when done like this. Hard coded because the first lab is the NULL time.
                        JSONString += " allDay: true"; // just to be sure even though this is the default
                        JSONString += "},";
                    }
                    else
                    {// otherwise make a JSON string for each of the days listed with that section
                        foreach (char day in currClass.days)
                        {
                            switch (day)
                            {
                                case 'M':
                                    moveDay = 0;
                                    break;
                                case 'T':
                                    moveDay = 1;
                                    break;
                                case 'W':
                                    moveDay = 2;
                                    break;
                                case 'R':
                                    moveDay = 3;
                                    break;
                                case 'F':
                                    moveDay = 4;
                                    break;
                                case 'S':
                                    moveDay = 5;
                                    break;
                                case 'N':
                                    moveDay = 6;
                                    break;
                            }

                            JSONString += "{";
                            JSONString += " title: '" + currClass.dept + " " + currClass.number.ToString() + " - " + currClass.sectionType + "',";
                            JSONString += " start: '" + schedDate.AddDays(moveDay).ToString("yyyy-MM-dd") + " " + currClass.startTime.ToString("HH:mm:ss") + "',";
                            JSONString += " end: '" + schedDate.AddDays(moveDay).ToString("yyyy-MM-dd") + " " + currClass.endTime.ToString("HH:mm:ss") + "',";
                            JSONString += " allDay: false";
                            JSONString += "},";
                        }
                    }
                }
                
                count += 7;
            }
            JSONString += "]";

            hiddenScheduleJSON.Value = JSONString;
            hiddenScheduleCount.Value = schedToReturn.possibleScheds.Count().ToString();
           
            
        }
        public void saveSchedules(object sender, EventArgs e)
        {
            try
            {
                //need if username exists in table, then do an update query rather than an insert.
                //also constrain username to be a unique value in the table
                string userName = Membership.GetUser().UserName;

                MySqlConnection con = new MySqlConnection(conStr);
                con.Open();
                string verifyQuery = "SELECT user_name FROM user_schedules WHERE user_name = @username";
                MySqlCommand cmdVerify = new MySqlCommand(verifyQuery, con);
                cmdVerify.Parameters.AddWithValue("@username", userName);
                MySqlDataReader verifyReader = cmdVerify.ExecuteReader();
                if (!verifyReader.HasRows)
                {
                    verifyReader.Close();
                    string mySchedules = hiddenScheduleJSON.Value;
                    string myCount = hiddenScheduleCount.Value;
                    int i = 0;
                    //insert into sql database here

                    string sqlQuery = "INSERT INTO user_schedules (user_name,sched_json,count) VALUES (@username,@json,@count)";
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, con);
                    cmd.Parameters.AddWithValue("@username", userName);
                    cmd.Parameters.AddWithValue("@json", mySchedules);
                    cmd.Parameters.AddWithValue("@count", myCount);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    verifyReader.Close();
                    string mySchedules = hiddenScheduleJSON.Value;
                    string myCount = hiddenScheduleCount.Value;
                    int i = 0;
                    //insert into sql database here

                    string sqlQuery = "UPDATE user_schedules SET sched_json=@json,count=@count WHERE user_name=@username";
                    MySqlCommand cmd = new MySqlCommand(sqlQuery, con);
                    cmd.Parameters.AddWithValue("@username", userName);
                    cmd.Parameters.AddWithValue("@json", mySchedules);
                    cmd.Parameters.AddWithValue("@count", myCount);
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch(Exception ex)//handle username null exception
            {
                //something about not being logged in should be handled here
                if (ex is NullReferenceException)
                {
                    Session["ErrorMessage"] = "You must be logged in to save schedules.";
                    Response.Redirect("Account/Login.aspx");
                }
                else
                {
                    throw;
                }
            }
        }
        public void retrieveSchedules()
        {
            //If there no username, we get a NullReferenceException
            //if there is no schedule to retrieve, then we get a MySqlException
            try
            {    
                string userName = Membership.GetUser().UserName;
                //query database for saved schedule data
                MySqlConnection con = new MySqlConnection(conStr);
                con.Open();
                string sqlQuery = "SELECT * FROM user_schedules WHERE user_name=@username";
                MySqlCommand cmd = new MySqlCommand(sqlQuery, con);
                cmd.Parameters.AddWithValue("@username", userName);
                MySqlDataReader schedReader = cmd.ExecuteReader();
                schedReader.Read();
                string retJSON = schedReader.GetValue(2).ToString();
                string retCount = schedReader.GetValue(3).ToString();
                hiddenScheduleJSON.Value = retJSON;
                hiddenScheduleCount.Value = retCount;
                //hiddenScheduleCount.Value = "5";
                schedReader.Close();
                con.Close();
            }
            catch(Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    Session["ErrorMessage"] = "You must be logged in to retrieve schedules.";
                    Response.Redirect("Account/Login.aspx");
                }
                else if (ex is MySqlException)
                {
                    Session["ErrorMessage"] = "Error! You do not have any saved schedules.";
                    Response.Redirect("Default.aspx");
                }
                else
                {
                    throw;
                }
            }
        } 
    }
}