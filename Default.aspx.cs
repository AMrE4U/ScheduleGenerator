using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI.HtmlControls;


namespace ScheduleGeneratorProject
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                message.Text = (string)(Session["ErrorMessage"]);
                Session.Contents.Remove("ErrorMessage");
            }
        }
    }
}