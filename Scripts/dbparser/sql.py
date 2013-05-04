#!/usr/bin/python
import types
import time
from datetime import datetime
f=open('mastercourses.txt', 'r')
fout = open('mastercourses.sql','w')

f.readline()
f.readline()

for line in f:
  line = line.rstrip()
  l = line.split("\t")
  dept = l[0]
  if(dept == ""):
    continue
  comnum = l[1]
  coursetitle  = l[3]
  coursetitle = coursetitle.replace("'", "''")
  if("XXXX" in comnum):
    continue
  classnum = l[5]
  #if(sectnum == ""):
  #  sectnum = "NULL"
  instructor = l[16]
  instructor = instructor.replace("'", " ")
  instructor = instructor.replace('"',"'")
  if(instructor == "" or instructor == "XXXX"):
    instructor = "''"
  credithours = l[7]
  seatsleft = l[9]
  component = l[13]
  starttime = l[17]
  class_begins = ""
  class_ends = ""
  seventhirty = 73000 #time.strptime("7:30:00", "%I:%M:%S")
  noon = 120000 #time.strptime("12:00:00", "%I:%M:%S")
  six = 60000 #time.strptime("6:00:00","%I:%M:%S")
  one = 10000 #time.strptime("1:00:00", "%I:%M:%S")
  
  if(starttime == "APPT"):
	starttime = "NULL"
  else:
	starttime = starttime + ":00"
	stnum = starttime.split(':')
	ststr = str(stnum[0]) + str(stnum[1]) + str(stnum[2])
	stime = int(ststr)
	#stime = time.strptime(starttime,"%I:%M:%S")
	
  endtime = l[18]
  if(endtime == ""):
	endtime = "NULL"
  else:
	endtime = endtime[0:5] + ":00 " + endtime[6:]
	etime = time.strptime(endtime, "%I:%M:%S %p")
  if (starttime != "NULL"):
    if ((stime > seventhirty) and (stime < noon)):
      starttime = starttime + " AM"
    if (stime >= noon):
      starttime = starttime + " PM"
    if ((stime >= one) and (stime < six)):
      starttime = starttime + " PM"
    if((stime >= six) and (stime <= seventhirty)):
      if (("AM" in endtime) or ((("Organic Chemistry" in coursetitle) or ("Organc Chemistry" in coursetitle)) and ("12:20" in endtime))):
        starttime = starttime + " AM"
      else:
        starttime = starttime + " PM"


  #if(coursetitle == ""):
    try:
	  starttime = datetime.strptime(starttime,"%I:%M:%S %p")
    except:
	  starttime = datetime.strptime(starttime,"%I:%M:%S")
    starttime = starttime.strftime('%H:%M:%S')
    endtime = datetime.strptime(endtime,"%I:%M:%S %p")
    endtime = endtime.strftime('%H:%M:%S')

  days = l[19]
  if(days == ""):
	days = "NULL"
  location = l[22]
  if(location == ""):
	location = "NULL"
  try:
    room = l[23]
  except:
	room = "NULL"
  outstr = "INSERT INTO mcourses (dept,comnum,coursetitle,classnum,credithours,seatsleft,component,instructor,starttime,endtime,days,location,room) VALUES ('" + dept + "'," + comnum + ",'" + coursetitle + "'," + classnum + "," + credithours +  "," + seatsleft + ",'" + component + "'," + instructor + ",'" + starttime + "','" + endtime + "','" + days + "','" + location + "','" + room +"');\n"
  
  fout.write(outstr)
  
f.close()
fout.close()
