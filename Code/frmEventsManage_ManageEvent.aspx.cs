using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

public partial class frmEventsManage_ManageEvent : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void meSubmitDateBTN_Click(object sender, EventArgs e)
    {
        DateTime startDate = new DateTime();

        try
        {
            startDate = (DateTime) meStartDateDP.SelectedDate;
        }

        catch(InvalidOperationException ex)
        {
            showPopup("Select a valid date.");
        }

        String eventName = meEventNameTB.Text;

        //-----------------Validate input---------------------------------
        if (eventName.Length <= 4)
        {
            showPopup("Please check the input.");
            return;
        }

        if(checkIfEventExists(eventName, startDate))
        {

        }

    }

    //===================================Database Operations=======================================

    private bool checkIfEventExists(String eventName, DateTime startDate)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM EventMaster WHERE EventName = @EN AND StartDate = @SD", connection);
            cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
            cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
            int count = (int)cmd.ExecuteScalar();
            cmd.Dispose();
            return count > 0;
        }
    }





    //===================================Utilities================================================
    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}