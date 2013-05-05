<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="results.aspx.cs" Inherits="ScheduleGeneratorProject.results" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel='stylesheet' type='text/css' href='Content/fullcalendar.css' />
    <script type='text/javascript' src='Scripts/fullcalendar.js'></script>
    <script>
        //Get the current date.
        var today = new Date();
        var schedCount;
        var currCount = 0;

        $(document).ready(function () {
            // page is now ready, initialize the calendar...

            //Get the count of schedules.
            schedCount = parseInt($('#MainContent_hiddenScheduleCount').val());

            //Dont initialize the calendar if there are no schedules.
            if (schedCount != 0) {
                $('#calendar').fullCalendar({
                    // put your options and callbacks here
                    header: {
                        left: 'prev,next',
                        center: 'title',
                        right: ''
                    },
                    titleFormat: '\'Schedules\'',
                    firstDay: 1,
                    height: 1200,
                    columnFormat: {
                        week: 'ddd'
                    },
                    allDayText: 'Any-Time',
                    minTime: 6,
                    maxTime: 23,
                    defaultView: 'agendaWeek',
                    events: eval($('#MainContent_hiddenScheduleJSON').val())
                })

                //Start with previous button hidden.
                $('.fc-button-prev').hide();

                //Hide next button if only one schedule.
                if (schedCount == 1) { $('.fc-button-next').hide(); }


                //Event listeners on previous and next buttons to hide them based 
                //on week being displayed.
                //Date comparison didn't work well, use a counter instead?
                $('.fc-button-prev').click(function () {
                    currCount--;
                    if (currCount <= 0) {
                        $('.fc-button-prev').hide();
                    }

                    if (currCount < schedCount) {
                        $('.fc-button-next').show();
                    }
                });

                $('.fc-button-next').click(function () {
                    currCount++;
                    if (currCount > 0) {
                        $('.fc-button-prev').show();
                    }

                    if (currCount >= schedCount - 1) {
                        $('.fc-button-next').hide();
                    }
                });

                $('.fc-view-agendaweek > div > div').css('overflow-y', 'hidden');
                $('.fc-agenda-gutter').css('width', 0);
                $('.fc-view-basicday').css('color', '#fff');
                $('.fc-view-basicday').css('background', '#fff');
                $('.fc-today').css('color', '#fff');
                $('.fc-today').css('background', '#fff');
                $('.fc-content').css('background', '#fff');
                $('#calendar').fullCalendar('render');
            }
            else { //There were zero schedules display message.
                $('#calendar').text("There were no schedules possible with the selected classes. Please try again.");
                $('#MainContent_saveBtn').hide();
            }
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="message" runat="server" CssClass="message-error" />
    <br />
    <asp:Button style="float:right" runat="server" ID="saveBtn" OnClick="saveSchedules" Text="Save Schedules" />
    <div id='calendar'></div>
    <asp:HiddenField ID="hiddenScheduleJSON" runat="server" />
    <asp:HiddenField ID="hiddenScheduleCount" runat="server" />
    

</asp:Content>
