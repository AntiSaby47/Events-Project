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
using System.Diagnostics;
using FtpClient;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

public partial class frmRegisterEventSchool : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;
    private static String parentID;
    private static String mode; //Event or Sub Event. Choosen through Combo box
    private string folderOnFTPServer = "test";
    private static int id = -47;



    protected void resETypeCombo_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        mode = resETypeCombo.SelectedItem.Text;
        if ( resETypeCombo.SelectedItem.Text == "Event" )
        {
            resSEventPanel.Visible = false;
            resEventPanel.Visible = true;
        }
        else if ( resETypeCombo.SelectedItem.Text == "Sub Event" )
        {
            resEventPanel.Visible = false;
            resSEventPanel.Visible = true;
            loadEvents();
        }
    }
    private void loadEvents()
    {
        resENameCombo.Items.Clear(); //Clear the Combo items to make sure duplicates are not made
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("SELECT id,EventName,StartDate,EndDate FROM dbo.EventMaster WHERE HasCategories = 1 AND EventStatus = 2", connection);
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
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading events!");
        }
    }

    protected void resRegisterBtn_Click(object sender, EventArgs e)
    {
        String eventName = null;
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        String organizedBy = null;

        if (mode == "Event")
        {
            eventName = resENametxt.Text.Trim();
            if (eventName.Length < 3)
            {
                showPopup("Enter a valid Event name!");
                return;
            }

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
        else if (mode == "Sub Event")
        {
            eventName = resSENameTxt.Text.Trim();
            if (eventName.Length < 3)
            {
                showPopup("Enter a valid Sub Event name!");
                return;
            }

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

        organizedBy.Trim();

        if (organizedBy.Length < 3)
        {
            showPopup("Check Input - Organized by");
            return;
        }
           
        int i = checkIfAlreadyRegistered(eventName, startDate, endDate, organizedBy);
        if(i > 0)
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
        int isActive;

        //Check this blocks
        if (parentID == null) //If Event
            isActive = 2;
        else
            isActive = 0;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(EventName, StartDate, EndDate, EventStatus, OrganisedBy,ParentEventID) VALUES(@EN, @SD, @ED, @AT, @OB,@pID)", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@AT", SqlDbType.SmallInt).Value = isActive;
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


    protected void resStatusRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "UploadExcel")
        {
            GridDataItem item = e.Item as GridDataItem;
            try
            {
                id = Int32.Parse(item["id"].Text);
                Debug.WriteLine("ID;"  + id);
            }
            catch (System.FormatException ex)
            {
                showPopup("Something went wrong while parsing Event ID. Please try again.");
            }
            
            if (checkIfExcelUploaded(id))
                showPopup("Excel already uploaded!");
            else
                resPopUp.Show();
        }
    }

    protected void ButtonOk_Click(object sender, EventArgs e)
    {
        //after certificate is uploaded and 'Ok' is clicked in pop up.
        try
        {
            string fileName = resImageFU.FileName;
            FtpService ftpClient = new FtpService();
            FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();

            byte[] fileData = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                using (BinaryReader binaryReader = new BinaryReader(resImageFU.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes(resImageFU.PostedFile.ContentLength);
                }
            }

            fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[^a-zA-Z0-9.]", "_"); //special characters in filename replaced with '_'
            fileName = fileName.Replace(' ', '_'); // White spaces in filename replaced with '_'.
            string fullPath = credentials.Url + folderOnFTPServer + "/" + fileName;

            string result = ftpClient.UploadFile(folderOnFTPServer, fileName, fileData, credentials);

            if (result.Trim().StartsWith("226 Successfully transferred"))
            {
                Debug.WriteLine("Uploaded");
            }
            else if (result.Trim() == "File Already Exists")
            {
                showPopup("An Excel file already exists with the same name. Please rename the file and try again.");
                return;
            }
            else
            {
                Debug.WriteLine(result);
                showPopup("Something went wrong while uploading the file. Please try again.");
                return;
            }

            Debug.WriteLine("ID: "+id);
            String query = "UPDATE EventMaster SET " +
                    "ExcelName = @ExcelName, " +
                    "EventStatus = 3 " +
                    "WHERE ID = @ID";

            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@ExcelName", SqlDbType.VarChar).Value = fileName;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                Debug.WriteLine(command.CommandText);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
                command.Dispose();
            }

            if (rowsAffected <= 0)
            {
                showPopup("Something went wrong while uploading the file. Please try again. [2]");
                return;
            }
            else
            {
                showPopup("File Uploaded!");
                Response.Redirect(Request.RawUrl);
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            showPopup("Something went wrong. Please try again.");
            return;
        }
    }

    protected void resStatusRG_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = e.Item as GridDataItem;
            int eventStatus = Int32.Parse(item["EventStatus"].Text);
            if (eventStatus != 2)
            {
                RadButton uploadBtn = item.FindControl("resExcelBtn") as RadButton;
                uploadBtn.Visible = false;
            }
        }
    }

    private bool checkIfExcelUploaded(int id)
    {
        //check if excel is already uploaded.
        string query = "SELECT ExcelName FROM EventMaster " +
            "WHERE ID = @ID";

        DataTable data = new DataTable();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand selectCommand = new SqlCommand(query, connection);
            selectCommand.Parameters.AddWithValue("@ID", id);
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
            connection.Open();
            adapter.Fill(data);
            selectCommand.Dispose();
        }

        if (data.Rows.Count == 1)
        {
            if (data.Rows[0]["ExcelName"] == System.DBNull.Value)
                return false;
            else
                return true;
        }

        return false;
    }

    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

    
}
