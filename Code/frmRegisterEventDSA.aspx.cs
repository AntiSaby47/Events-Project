using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using Telerik.Web.UI;

public partial class frmRegisterEventDSA : System.Web.UI.Page
{
    String connectionString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;

    protected void resERegisterBtn_Click(object sender, EventArgs e)
    {
        String eventName = redENametxt.Text.Trim();
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        String organizedBy = redEOrganizedBy.Text.Trim();
        int allowSubEvents = 0;
        if (redAllowSubEventCB.Checked) allowSubEvents = 1; else allowSubEvents = 0;

        if(eventName.Length < 4)
        {
            showPopup("Invalid Event Name!");
            return;
        }

        if (organizedBy.Length < 3)
        {
            showPopup("Invalid Input - Organized by");
            return;
        }

        try
        {
            startDate = (DateTime)redESDateDP.SelectedDate;
            endDate = (DateTime)redEEDateDP.SelectedDate;
        }
        catch (InvalidOperationException ex)
        {
            showPopup("Select a valid date.");
        }

        if (checkIfAlreadyRegistered(eventName, startDate, endDate, organizedBy) > 0)
        {
            showPopup("Event already registered");
            return;
        }

        int i = checkIfAlreadyRegistered(eventName, startDate, endDate, organizedBy);
        if (i > 0)
        {
            showPopup("Event already registered");
            return;
        }

        registerEvent(eventName, startDate, endDate, organizedBy, allowSubEvents);
    }
    private int checkIfAlreadyRegistered(String eventName, DateTime startDate, DateTime endDate, string organizedBy)
    {
        try 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT ID FROM dbo.EventMaster WHERE EventName=@EN AND StartDate=@SD AND EndDate=@ED AND OrganisedBy=@OB", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                int id = (int)cmd.ExecuteScalar();
                cmd.Dispose();
                return id;
            }
        }
        catch (NullReferenceException)
        {
            return -1;
        }
    }
    private void registerEvent(String eventName, DateTime startDate, DateTime endDate, string organizedBy, int allowSubEvents)
    {
        int isActive = 2;
        try 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(EventName, StartDate, EndDate, Active, OrganisedBy, HasCategories) VALUES(@EN, @SD, @ED, @AT, @OB, @aSE)", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@AT", SqlDbType.SmallInt).Value = isActive;
                cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                cmd.Parameters.Add("@aSE", allowSubEvents);
                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected <= 0)
                {
                    showPopup("Couldn\\'t register event. Please reopen the page and try again.");
                }

                else
                {
                    showPopup("Event registered. Note down the Event name and start date. You\\'ll need it while uploading event data.");
                }
                cmd.Dispose();
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine("Ex: " + ex.Message);
            showPopup("Something went wrong while registering the event!");
            return;
        }
    }

    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

    protected void redSERegRequestRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        try
        {
            String ID,pID;
            if (e.CommandName == "Approve" || e.CommandName == "Reject")
            {
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;
                pID = item["parentEventID"].Text;
                String query = null;
                int status = 0;
                if (e.CommandName == "Approve")
                    status = 2;
                if (e.CommandName == "Reject")
                    status = -1;
                query = "UPDATE dbo.EventMaster SET Active=@ST WHERE id=@ID AND ParentEventID=@PID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@ST", SqlDbType.Int).Value = status;
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    command.Parameters.Add("@PID", SqlDbType.Int).Value = pID;
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                    {
                        Response.Write("<script>alert('Something went wrong. Please try again.');</script>");
                    }
                    else
                    {
                        Response.Redirect(Request.RawUrl);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Ex: " + ex.Message);
            showPopup("Something went wrong while performing the operation!");
            return;
        }

    }
}