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

public partial class frmEventsManage_RegisterEvent : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void reSubmitBTN_Click(object sender, EventArgs e)
    {
        //----------Get all the values-------------------------------
        DateTime startDate, endDate;
        String eventName = reEventNameTB.Text;
        String participationYear = reParticipationYearTB.Text;
        String certificateType = reCertificateTypeDDL.SelectedValue;

        try
        {
            startDate = (DateTime)reStartDateDP.SelectedDate;
            endDate = (DateTime)reEndDateDP.SelectedDate;
        }
        catch(InvalidOperationException ex)
        {
            showPopup("Enter a valid date.");
            return;
        }

        int isActive = reIsActiveCB.Checked ? 1 : 0;
        String organizedBy = reOrganizedByTB.Text;
        //-----------------------------------------------------------

        //----------Validate Input-----------------------------------
        if(eventName.Length <= 4 || participationYear.Length != 4 || organizedBy.Length <= 5)
        {
            showPopup("Please check the input.");
            return;
        }

        if(startDate.Date > endDate.Date)
        {
            showPopup("Event end date can\\'t be less than start date.");
            return;
        }
        //-----------------------------------------------------------
        if (!checkIfEventExists(eventName, startDate))
            registerEvent(eventName, certificateType, participationYear, startDate, endDate, isActive, organizedBy);
        else
            showPopup("Event already registered.");
    }

    //---------------------Database Operations-------------------------------------

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


    private void registerEvent(String eventName, String certificateType, String participationYear, DateTime startDate, DateTime endDate, int isActive, String organizedBy)
    {
        Debug.WriteLine("Event: " + eventName + "  Type: " + certificateType + "  Year: " + participationYear + "  StartDate: " + startDate.ToString() + "  EndDate: " + endDate.ToString() + "  Is Active: " + isActive + "  Organized by: " + organizedBy);
        using(SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(CertificateType, EventName, ParticipationYear, StartDate, EndDate, Active, OrganisedBy) VALUES(@CT, @EN, @PY, @SD, @ED, @AT, @OB)", connection);
            cmd.Parameters.Add("@CT", SqlDbType.VarChar).Value = certificateType;
            cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
            cmd.Parameters.Add("@PY", SqlDbType.VarChar).Value = participationYear;
            cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
            cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
            cmd.Parameters.Add("@AT", SqlDbType.Bit).Value = isActive;
            cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
            int rowsAffected = cmd.ExecuteNonQuery();
            if(rowsAffected <= 0)
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


    //--------------------Other Utilities---------------------------------------------
    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}