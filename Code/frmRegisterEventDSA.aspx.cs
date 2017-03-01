using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public partial class frmRegisterEventDSA : BasePage
{
    String connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void resERegisterBtn_Click(object sender, EventArgs e)
    {
        String eventName = redENametxt.Text;
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        String organizedBy = redEOrganizedBy.Text;
        int allowSubEvents = 0;
        if (redAllowSubEventCB.Checked) allowSubEvents = 1; else allowSubEvents = 0;
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
        int isActive = 1;
        try 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(EventName, StartDate, EndDate, Active, OrganisedBy, HasCategories) VALUES(@EN, @SD, @ED, @AT, @OB, @aSE)", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@AT", SqlDbType.Bit).Value = isActive;
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
            return;
        }
    }
    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}