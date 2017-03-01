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
using Telerik.Web.UI;

public partial class frmRegisterEventSchool : System.Web.UI.Page
{
    String connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    private static String parentID;
    protected void resETypeCombo_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if ( resETypeCombo.SelectedItem.Text == "Event" )
        {
            resSEventPanel.Visible = false;
            resEventPanel.Visible = true;
            RadButton1.Text = "Register Event";
            resEventPanel.Controls.Add(RadButton1);
        }
        else if ( resETypeCombo.SelectedItem.Text == "Sub Event" )
        {
            resEventPanel.Visible = false;
            resSEventPanel.Visible = true;
            RadButton1.Text = "Register Event Category";
            resSEventPanel.Controls.Add(RadButton1);
            loadEvents();
        }
    }
    private void loadEvents()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("SELECT id,EventName,StartDate,EndDate FROM dbo.EventMaster  WHERE HasCategories = 1;", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                DataTable types = new DataTable();
                adapter.Fill(types);

                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);

                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["id"].ToString();
                    parentID = row["id"].ToString();
                    String sqlFormattedstartDate = ((DateTime)row["StartDate"]).ToString("yyyy-MM-dd");
                    String sqlFormattedEndDate = ((DateTime)row["EndDate"]).ToString("yyyy-MM-dd");
                    String text = row["EventName"] + " :: " + sqlFormattedstartDate + " to " + sqlFormattedEndDate;

                    itemData.Text = text;
                    resENameCombo.Items.Add(itemData);
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Something went wrong while loading certificate types.');</script>");
        }
    }

    protected void RadButton1_Click(object sender, EventArgs e)
    {
        String eventName = null;
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        String organizedBy = null;

        if (RadButton1.Text == "Register Event")
        {
            eventName = resENametxt.Text;
            try
            {
                startDate = (DateTime)resESDateDP.SelectedDate;
                endDate = (DateTime)resESDateDP.SelectedDate;
            }
            catch (InvalidOperationException ex)
            {
                showPopup("Select a valid date.");
            }
            organizedBy = resEOrganizedBy.Text;
            parentID = null;
        }
        if (RadButton1.Text == "Register Event Category")
        {
            eventName = resSENameTxt.Text;
            String data = null;

            String St = resENameCombo.SelectedItem.Text;
            //-----------Calculating start date--------------------
            int pFrom = St.IndexOf(" :: ") + " :: ".Length;
            int pTo = St.LastIndexOf(" to ");
            startDate = Convert.ToDateTime(St.Substring(pFrom, pTo - pFrom));
            //-----------Calculating end date-----------------------
            string tobesearched = " to ";
            if (pTo != -1)
            { data = St.Substring(pTo + tobesearched.Length); }
            endDate = Convert.ToDateTime(data);
            parentID = resENameCombo.SelectedItem.Value;
            organizedBy = resSEOrganizedByTxt.Text;
        }

        int i = checkIfAlreadyRegistered(eventName, startDate, endDate, organizedBy);
        System.Diagnostics.Debug.WriteLine("iiiiiiiiiiiiiiiiiiiiiiiiii" + i);
        if(i>0)
        {
            showPopup("Event already registered");
            return;
        }
        registerEvent(eventName, startDate, endDate, organizedBy, parentID);
    }
    private int checkIfAlreadyRegistered(String eventName, DateTime startDate, DateTime endDate, string organizedBy)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                System.Diagnostics.Debug.WriteLine("aaaaaaaaaa" + eventName + "  " + startDate + "" + endDate + "" + organizedBy);
                SqlCommand cmd = new SqlCommand("SELECT ID FROM dbo.EventMaster WHERE EventName=@EN AND StartDate=@SD AND EndDate=@ED AND OrganisedBy=@OB", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                int id = (int)cmd.ExecuteScalar();
                cmd.Dispose();
                return id;
            }
            catch (NullReferenceException)
            {
                return -1;
            }
        }
    }

    private void registerEvent(String eventName, DateTime startDate, DateTime endDate, string organizedBy, String parentID)
    {
        int isActive = 0;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            {
                SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(EventName, StartDate, EndDate, Active, OrganisedBy,ParentEventID) VALUES(@EN, @SD, @ED, @AT, @OB,@pID)", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@AT", SqlDbType.Bit).Value = isActive;
                cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                if (!String.IsNullOrEmpty(parentID))
                {
                    cmd.Parameters.AddWithValue("@pID", parentID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@pID", DBNull.Value);
                }
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
    }

    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

}
