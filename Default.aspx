<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ScheduleGeneratorProject._Default" %>

<asp:Content runat="server" ID="HeadContent" ContentPlaceHolderID="HeadContent">    
    
       
    <script type="text/javascript" src="Scripts/jquery.tokeninput.js"></script>
    <link rel="stylesheet" type="text/css" href="Content/token-input.css" />
    
    
    <script type="text/javascript" src="Scripts/jquery-ui-1.10.2.custom.js"></script>
    <link rel="stylesheet" type="text/css" href="Content/jquery-ui-1.10.2.custom.css" />
    <style>
        #sortable1, #sortable2 { list-style-type: none; margin: 0; padding: 0; float: left; margin-right: 10px; }
        #sortable1 li, #sortable2 li { margin: 0 5px 5px 5px; padding: 5px; font-size: 1.2em; width: 150px; }
        .mornAft { border:2px solid red }
        .days { border:2px solid blue }
        .timeGap { border:2px solid #FFDA00 }
        .daysoff { border:2px solid black }
    </style>
    
    <script type="text/javascript">
        var courseList = new Array();
        var preferences = new Array();
        function setHidden() {
            var prefIDs = $('#sortable2').sortable('toArray');
            $('#MainContent_hiddenCourseList').val(courseList.toString());
            $('#MainContent_hiddenPreferences').val(prefIDs.toString());
        }
        function markSaved() {
            $('#MainContent_hiddenSavedSchedule').val("savedSchedule");
        }
        $(document).ready(function () {
            //Need to add an onRemove event and check for null list of courses
            $('#courseName').tokenInput("http://schedgen.dlinkddns.com/coursenameJSON", {
                onAdd: function (item) {                  
                    courseList.push(item.name);
                    document.getElementById('FeaturedContent_message').style.display = 'none';
                },
                onDelete: function (item) {                  
                    courseList.splice(courseList.indexOf(item.name), 1);
                },
                preventDuplicates: true
            });
            $('#sortable1').sortable({
                connectWith: ".connectedSortable",                
                items: "li:not(.ui-state-disabled)",
                receive: function (event, ui) {
                    for (var i = 0; i < preferences.length; i++) {
                        if ($(ui.item).hasClass(preferences[i])) {
                            preferences.splice(i, 1);
                        }
                    }
                }
            }).disableSelection();
            $('#sortable2').sortable({
                connectWith: ".connectedSortable",
                items: "li:not(.ui-state-disabled)",
                receive: function (event, ui) {
                    var arr = ["mornAft", "days", "timeGap"];
                    for (var i = 0; i < arr.length; i++) {
                        if ($(ui.item).hasClass(arr[i])) {
                            if ($.inArray(arr[i], preferences) != -1) {
                                $(ui.sender).sortable('cancel');                             
                            }
                            else {
                                preferences.push(arr[i]);
                            }                            
                        }
                    }
                }
            }).disableSelection();
            $("#instructions").click(function () {
                $("#toggleInst").toggle("blind", 500, function () {
                    if ($(this).is(':hidden')) {
                        $("#instSpan").text("Instructions [+]");
                    }
                    else {
                        $("#instSpan").text("Instructions [-]");
                    }
                });
                return false;
            });
        });
    </script>

</asp:Content>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
     
    <section class="featured">
        <div class="content-wrapper" style="min-height:330px">
            <table>
                <tr>
                    <td rowspan="3" style="width:60%; vertical-align:top; padding-right:5%">
                        <hgroup class="title">
                            <h1><%: Title %>.</h1>
                            <h2>Select options below to generate a class schedule.</h2>
                        </hgroup>
                        <p>
                            <input type="text" id="courseName">                
                        </p>
                        <asp:Button runat="server" ID="submitBtn" PostBackUrl="~/results.aspx" OnClientClick="setHidden()" Text="Generate Schedules" />
                        <asp:Button runat="server" ID="retBtn" PostBackUrl="~/results.aspx" OnClientClick="markSaved()" Text="My Saved Schedules" />
                        <br />
                        <asp:Label ID="message" runat="server" CssClass="message-error" />
                        <br />
                        <a id="instructions" style="text-decoration:none">
                            <span id="instSpan" style="color:white; font-size:1.3em; font-family:'Segoe UI', Verdana, Helvetica, sans-serif">
                                Instructions [+]
                            </span>
                        </a>
                        <div id="toggleInst" style="display:none">
                            <div style="margin-left:16px">
                                <span style="color:white; font-size:1em; font-family:'Segoe UI', Verdana, Helvetica, sans-serif">
                                    To begin using the schedule generator, start typing the department of the course you wish to take into the 
                                    text box above. The text box uses auto-complete to help you choose the class you're looking for, but you must 
                                    give it at least one letter. The courses have the form "[Department] [Course Number]" and are not case sensitive 
                                    (i.e. MATH 101). If you are not sure what the department or course number is for the course you want to take, 
                                    please visit <a href="https://classes.ku.edu">classes.ku.edu</a>. Once you have selected all of your desired courses, you may choose 
                                    preferences from the list to the right. Please note that these preferences are grouped by type, and only one 
                                    of each type can be selected. For instance, if you drag "Morning Classes" to the selected preferences list, 
                                    you cannot also choose "Afternoon Classes" without first dragging "Morning Classes" back. You can order the 
                                    selected preferences list from most important to least important by dragging a preference up or down the list. 
                                    Preferences higher on the list will be considered more important than those lower on the list. The "Include 
                                    Online/Appt" check box, if checked, will include courses that do not have specific days or times associated 
                                    with them (i.e. Online courses). If checked, these classes will be displayed in the "Any-time" section on the 
                                    calendar.
                                </span>
                            </div>
                        </div>
                    </td>
                    <td style="vertical-align:top">
                        <span style="color:white; font-size:1.3em; font-family:'Segoe UI', Verdana, Helvetica, sans-serif">Available Preferences</span>
                    </td>
                    <td style="vertical-align:top">
                        <span style="color:white; font-size:1.3em; font-family:'Segoe UI', Verdana, Helvetica, sans-serif">Selected Preferences</span>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align:top">
                        <ul id="sortable1" class="connectedSortable" style="border:2px solid white; height:320px; padding-top: 5px">
                            <li id="morn" class="ui-state-highlight mornAft">Morning Classes</li>
                            <li id="after" class="ui-state-highlight mornAft">Afternoon Classes</li>
                            <li id="night" class="ui-state-highlight mornAft">Evening Classes</li>
                            <li id="MWF" class="ui-state-highlight days">Days MWF</li>
                            <li id="TR" class="ui-state-highlight days">Days TR</li>
                            <li id="mostdaysoff" class="ui-state-highlight days">Most Days Off</li>
                            <li id="smGap" class="ui-state-highlight timeGap">Smallest Time Gap</li>
                            <li id="lgGap" class="ui-state-highlight timeGap">Largest Time Gap</li>                            
                        </ul>
                    </td>
                    <td style="vertical-align:top">
                        <ul id="sortable2" class="connectedSortable" style="border:2px solid white; height:320px; width:172px; padding-top: 5px">
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="vertical-align:top">
                        <span style="color:white; font-size:1.3em; font-family:'Segoe UI', Verdana, Helvetica, sans-serif">
                            <asp:CheckBox runat="server" ID="onlineCB" />
                            Include Online/Appt Courses
                        </span>
                    </td>
                </tr>
            </table>
        </div>        
    </section>
    
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        
    <asp:HiddenField ID="hiddenCourseList" runat="server" />
    <asp:HiddenField ID="hiddenPreferences" runat="server" />
    <asp:HiddenField ID="hiddenSavedSchedule" runat="server" />
  
</asp:Content>