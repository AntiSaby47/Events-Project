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
    private static string parentID;
    private static string mode, uploadDataMode; //Event or Sub Event. Choosen through Combo box
    private string folderOnFTPServer = "test";
    private static int id = -47;
    private static int refreshMode = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(refreshMode == 1)
        {
            showPopup("Event registered. Note down the Event name and start date. You\\'ll need it while uploading event data.");
            refreshMode = 0;
        }

        else if (refreshMode == 2)
        {
            showPopup("Could not find events/sub-events!");
            refreshMode = 0;
        }

        else if (refreshMode == 3)
        {
            showPopup("File Uploaded Successfully!");
            refreshMode = 0;
        }
    }

    protected void resETypeCombo_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        int index = resETypeCombo.SelectedIndex;
        mode = index == 0 ? "Event" : "Sub Event";
        if (mode == "Event")
        {
            resSEventPanel.Visible = false;
            resEventPanel.Visible = true;
        }
        else if (mode == "Sub Event")
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
                    string sqlFormattedstartDate = ((DateTime)row["StartDate"]).ToString("yyyy-MM-dd");
                    string sqlFormattedEndDate = ((DateTime)row["EndDate"]).ToString("yyyy-MM-dd");
                    string text = row["EventName"] + " :: " + sqlFormattedstartDate + " to " + sqlFormattedEndDate;
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
        string eventName = null;
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        string organizedBy = null;

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
                return;
            }
            organizedBy = resEOrganizedBy.Text;
            parentID = null;
        }
        else if (mode == "Sub Event")
        {
            if (resENameCombo.SelectedIndex < 0)
            {
                showPopup("Select an Event!");
                return;
            }

            eventName = resSENameTxt.Text.Trim();
            if (eventName.Length < 3)
            {
                showPopup("Enter a valid Sub Event name!");
                return;
            }

            string data = null;

            string St = resENameCombo.SelectedItem.Text;
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
    private int checkIfAlreadyRegistered(string eventName, DateTime startDate, DateTime endDate, string organizedBy)
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

    private void registerEvent(string eventName, DateTime startDate, DateTime endDate, string organizedBy, string parentID)
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
                SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(EventName, StartDate, EndDate, EventStatus, OrganisedBy,ParentEventID) VALUES(@EN, @SD, @ED, @AT, @OB, @pID)", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                cmd.Parameters.Add("@AT", SqlDbType.SmallInt).Value = isActive;
                cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                
                if (!string.IsNullOrEmpty(parentID))
                    cmd.Parameters.AddWithValue("@pID", parentID);
                else
                    cmd.Parameters.AddWithValue("@pID", DBNull.Value);
                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected <= 0)
                {
                    showPopup("Couldn\\'t register event. Please reopen the page and try again.");
                }

                else
                {
                    refreshMode = 1; //Show Event Registered Popup
                    Response.Redirect(Request.RawUrl);
                }
                cmd.Dispose();
            }

        }
    }


    protected void resStatusRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        
    }

    protected void resStatusRG_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = e.Item as GridDataItem;
            int eventStatus = Int32.Parse(item["EventStatus"].Text);
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

    private void showPopup(string text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

    protected void resDownloadFormatsBtn_Click(object sender, EventArgs e)
    {
        string fileName = "Events_Certificate_Formats.docx";
        FtpService ftpClient = new FtpService();
        FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();
        FtpWebResponse response = ftpClient.DowloadFile(folderOnFTPServer, fileName, FtpUserPassword.GetUMSFtpCredentials());

        using (MemoryStream stream = new MemoryStream())
        {
            response.GetResponseStream().CopyTo(stream);
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(stream.ToArray());
            Response.End();
        }
    }
    protected void resUDEventTypeCB_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        udrowEvent.Visible = udrowDDL.Visible = udrowFU.Visible = udrowBtn.Visible = udrowSubEvent.Visible = false;

        int index = resUDEventTypeCB.SelectedIndex;
        uploadDataMode = index == 0 ? "Event" : "Sub Event";
        if (uploadDataMode == "Event")
            udrowEvent.Visible = udrowDDL.Visible = udrowFU.Visible = udrowBtn.Visible = true;
        else if (uploadDataMode == "Sub Event")
            udrowEvent.Visible = udrowDDL.Visible = udrowFU.Visible = udrowBtn.Visible = udrowSubEvent.Visible = true;

        resUDEventCB.ClearSelection();
        resUDEventCB.Items.Clear();
        resUDSubEventCB.ClearSelection();
        resUDSubEventCB.Items.Clear();
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("SELECT id, EventName FROM EventMaster WHERE ParentEventID IS NULL AND EventStatus = 2", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                DataTable types = new DataTable();
                adapter.Fill(types);
                if (types.Rows.Count <= 0)
                {
                    refreshMode = 2;
                    Response.Redirect(Request.RawUrl);
                    return;
                }
                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);
                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["id"].ToString();
                    itemData.Text = row["EventName"].ToString();
                    resUDEventCB.Items.Add(itemData);
                }
            }
        }

        catch (Exception ex)
        {
            udrowEvent.Visible = udrowDDL.Visible = udrowFU.Visible = udrowBtn.Visible = udrowSubEvent.Visible = false;
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading events for uploading file!");
        }
    }


    protected void resUDEventCB_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        resUDSubEventCB.ClearSelection();
        resUDSubEventCB.Items.Clear();

        if (uploadDataMode == "Event")
            return;
        try
        {
            int eventID = Int32.Parse(resUDEventCB.SelectedValue);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("SELECT id, EventName FROM EventMaster WHERE ParentEventID = @PID AND EventStatus = 2", connection);
                selectCommand.Parameters.Add("@PID", SqlDbType.Int).Value = eventID;
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                DataTable types = new DataTable();
                adapter.Fill(types);
                if(types.Rows.Count <= 0)
                {
                    refreshMode = 2;
                    Response.Redirect(Request.RawUrl);
                    return;
                }
                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);
                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["id"].ToString();
                    itemData.Text = row["EventName"].ToString();
                    resUDSubEventCB.Items.Add(itemData);
                }
            }
        }

        catch(FormatException ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading sub-events for uploading file!");
        }
        
    }
    protected void resUDUploadBtn_Click(object sender, EventArgs e)
    {
        try
        {
            string eventID = resUDEventCB.SelectedValue;
            string subEventID = resUDSubEventCB.SelectedValue;
            string certificateType = null;
            int id = Int32.Parse(uploadDataMode == "Event" ? eventID : subEventID);
            certificateType = resUDCertFormatsDDL.SelectedValue;
            if (certificateType == "None")
            {
                showPopup("Select a Certificate Format!");
                return;
            }

            string fileName = resUDDataFileFU.FileName;
            if (String.IsNullOrEmpty(fileName))
            {
                showPopup("Select a file to upload!");
                return;
            }
            String ext = System.IO.Path.GetExtension(fileName);
            if (ext.ToLower() != ".xls" && ext.ToLower() != ".xlsx")
            {
                showPopup("Select a valid excel file to upload!");
                return;
            }

            fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[^a-zA-Z0-9.]", "_");
            fileName = fileName.Replace(' ', '_');
            fileName = id + "_" + certificateType + "_" + fileName;
            FtpService ftpClient = new FtpService();
            FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();
            byte[] fileData = null;
            using (BinaryReader binaryReader = new BinaryReader(resUDDataFileFU.PostedFile.InputStream))
            {
                fileData = binaryReader.ReadBytes(resUDDataFileFU.PostedFile.ContentLength);
            }

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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE EventMaster SET ExcelName = @EN WHERE ID = @ID AND EventStatus = 2", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = fileName;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected <= 0)
                    showPopup("Couldn\\'t upload file. Please reopen the page and try again.");
                else
                {
                    refreshMode = 3; //Show File Uploaded Popup
                    Response.Redirect(Request.RawUrl);
                }
                cmd.Dispose();
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while uploading the file.");
        }

    }

}
