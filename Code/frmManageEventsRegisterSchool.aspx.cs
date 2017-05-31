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
using Microsoft.ApplicationBlocks.Data;
using System.Data.OleDb;
using System.Data.Common;

public partial class frmRegisterEventSchool : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    System.Data.DataSet dataset = new System.Data.DataSet();
    private static string parentID;
    private static string mode, uploadDataMode; //Event or Sub Event. Choosen through Combo box
    private static int id = -47;
    private static int refreshMode = 0;

    //---------Keep check----------------------------
    private string folderOnFTPServer = "test";
    private string TEMP_FOLDER_PATH = "~/Temp/";
    //-----------------------------------------------


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SelectSchool.SelectedIndex = 0;
            loadSchools(SelectSchool);
        }
        if (refreshMode == 1)
        {
            showPopup("Event registered!");
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

        else if (refreshMode == 4)
        {
            showPopup("File data is not correct. Please check the data and upload the file again.");
            string[] filePaths = Directory.GetFiles(Server.MapPath(TEMP_FOLDER_PATH));
            foreach (string filePath in filePaths)
            {
                if (filePath.Contains("EC_TMP_"))
                    File.Delete(filePath);
            }
            refreshMode = 0;
        }

        //------Delete the temporary files---------------------------------
        string[] files = Directory.GetFiles(Server.MapPath(TEMP_FOLDER_PATH));
        foreach (string filePath in files)
        {
            if (filePath.Contains("EC_TMP_"))
                File.Delete(filePath);
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
            loadSchools(resEOrganizedBy);
        }
        else if (mode == "Sub Event")
        {
            resEventPanel.Visible = false;
            resSEventPanel.Visible = true;
            loadEvents();
            loadSchools(resSEOrganizedByCombo);
        }
    }

    private void loadEvents()
    {
        resENameCombo.Items.Clear(); //Clear the Combo items to make sure duplicates are not made
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pLoadDataEventCertificates]");
                DataTable types = new DataTable();
                types = dataset.Tables[0];

                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);

                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["id"].ToString();
                    parentID = row["id"].ToString();
                    string sqlFormattedstartDate = ((DateTime)row["StartDate"]).ToString("dd-MM-yyyy");
                    string sqlFormattedEndDate = ((DateTime)row["EndDate"]).ToString("dd-MM-yyyy");
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
    private void loadSchools(RadComboBox resEOrganizedBy)
    {
        resEOrganizedBy.Items.Clear(); //Clear the Combo items to make sure duplicates are not made
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pLoadDataEventCertificates]");
                DataTable types = new DataTable();
                types = dataset.Tables[1];

                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);

                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["DivisionSectionCode"].ToString();
                    string text = row["DivisionSectionCode"] + " :: " + row["Name"];
                    itemData.Text = text;
                    resEOrganizedBy.Items.Add(itemData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading schools!");
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
                endDate = (DateTime)resEEDateDP.SelectedDate;
            }
            catch (InvalidOperationException ex)
            {
                showPopup("Select a valid date.");
                return;
            }
            organizedBy = resEOrganizedBy.SelectedItem.Value;
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
            Debug.WriteLine(St.Substring(pFrom, pTo - pFrom));
            startDate = DateTime.ParseExact(St.Substring(pFrom, pTo - pFrom), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //-----------Calculating end date-----------------------
            string tobesearched = " to ";
            if (pTo != -1)
            { data = St.Substring(pTo + tobesearched.Length); }
            endDate = DateTime.ParseExact(St.Substring(pFrom, pTo - pFrom), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            parentID = resENameCombo.SelectedItem.Value;
            organizedBy = resSEOrganizedByCombo.SelectedItem.Value;
        }

        organizedBy.Trim();

        if (organizedBy.Length <= 0)
        {
            showPopup("Check Input - Organized by");
            return;
        }

        int i = checkIfAlreadyRegistered(eventName, startDate, endDate, organizedBy);
        if (i > 0)
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
                int hasCategories = 0;
                connection.Open();
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@eventName", eventName);
                param[1] = new SqlParameter("@startDate", startDate);
                param[2] = new SqlParameter("@endDate", endDate);
                param[3] = new SqlParameter("@eventStatus", DBNull.Value);
                param[4] = new SqlParameter("@organisedBy", organizedBy);
                param[5] = new SqlParameter("@parentID", DBNull.Value);
                param[6] = new SqlParameter("@hasCategories", hasCategories);

                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pRegisterEventEventCertificates]", param);
                int id = dataset.Tables[0].Rows.Count;
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
        if (startDate >= DateTime.Now)
        {
            if (endDate >= DateTime.Now)
            {
                if (startDate <= endDate)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        {
                            int hasCategories = 0;
                            int es = 0;
                            connection.Open();
                            SqlParameter[] param = new SqlParameter[7];
                            param[0] = new SqlParameter("@eventName", eventName);
                            param[1] = new SqlParameter("@startDate", startDate);
                            param[2] = new SqlParameter("@endDate", endDate);
                            param[3] = new SqlParameter("@eventStatus", es);
                            param[4] = new SqlParameter("@organisedBy", organizedBy);
                            if (!string.IsNullOrEmpty(parentID))
                                param[5] = new SqlParameter("@parentID", parentID);
                            else
                                param[5] = new SqlParameter("@parentID", null);
                            param[6] = new SqlParameter("@hasCategories", hasCategories);


                            dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pRegisterEventEventCertificates]", param);
                            int rowsAffected = dataset.Tables[1].Rows.Count;

                            if (rowsAffected <= 0)
                            {
                                showPopup("Couldn\\'t register event. Please reopen the page and try again.");
                            }

                            else
                            {
                                refreshMode = 1; //Show Event Registered Popup
                                Response.Redirect(Request.RawUrl);
                            }
                        }

                    }
                }
                else
                {
                    showPopup("Start Date Should be less than End Date!.");
                }
            }
            else
            {
                showPopup("End Date should be greater than today\\'s date!");
            }

        }
        else
        {
            showPopup("Start Date should be greater than today\\'s date!");
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
            string eventStatus = item["EventStatus"].Text;
        }
    }

    private bool checkIfExcelUploaded(int id)
    {
        //check if excel is already uploaded.
        DataTable data = new DataTable();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@organisedBy", null);
            param[1] = new SqlParameter("@ID", ID);
            dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pSchoolwiseDataEventCertificates]", param);
            data = dataset.Tables[2];

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

    protected void SelectSchool_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        try
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@organisedBy", SelectSchool.SelectedValue);
                param[1] = new SqlParameter("@ID", id);
                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pSchoolwiseDataEventCertificates]", param);
                table = dataset.Tables[3];

                resStatusRG.DataSource = table;
                resStatusRG.DataBind();
                resStatusRG.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while load data.");
        }

    }
    protected void resDownloadFormatsBtn_Click(object sender, EventArgs e)
    {
        try
        {
            string fileName = "Events_Certificate_Formats.doc";
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
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while downloading certificate formats!");
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
                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pLoadDataEventCertificates]");
                DataTable types = new DataTable();
                if (uploadDataMode == "Event")
                    types = dataset.Tables[2];
                else
                    types = dataset.Tables[3];

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

                    string sqlFormattedstartDate = ((DateTime)row["StartDate"]).ToString("dd-MM-yyyy");
                    string sqlFormattedEndDate = ((DateTime)row["EndDate"]).ToString("dd-MM-yyyy");
                    string text = row["EventName"] + " :: " + sqlFormattedstartDate + " to " + sqlFormattedEndDate;
                    itemData.Text = text;

                    //itemData.Text = row["EventName"].ToString();
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
                SqlCommand selectCommand = new SqlCommand();
                selectCommand.CommandText = string.Format("SELECT id, EventName FROM {0} WHERE ParentEventID = @PID AND EventStatus = 2", "EventMaster");
                selectCommand.Connection = connection;
                selectCommand.Parameters.Add("@PID", SqlDbType.Int).Value = eventID;
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
                    resUDSubEventCB.Items.Add(itemData);
                }
            }
        }

        catch (FormatException ex)
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
            //string certificateType = null;
            int id = Int32.Parse(uploadDataMode == "Event" ? eventID : subEventID);
            //certificateType = resUDCertFormatsDDL.SelectedValue;
            //if (certificateType == "None")
            //{
            //    showPopup("Select a Certificate Format!");
            //    return;
            //}

            string fileName = resUDDataFileFU.FileName;
            if (String.IsNullOrEmpty(fileName))
            {
                showPopup("Select a file to upload!");
                return;
            }
            String fileExtension = System.IO.Path.GetExtension(fileName);
            if (fileExtension.ToLower() != ".xls" && fileExtension.ToLower() != ".xlsx")
            {
                showPopup("Select a valid excel file to upload!");
                return;
            }

            fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[^a-zA-Z0-9.]", "_");
            fileName = fileName.Replace(' ', '_');
            //fileName = id + "_" + certificateType + "_" + fileName;
            fileName = id + "_" + fileName;
            FtpService ftpClient = new FtpService();
            FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();
            byte[] fileData = null;
            using (BinaryReader binaryReader = new BinaryReader(resUDDataFileFU.PostedFile.InputStream))
            {
                fileData = binaryReader.ReadBytes(resUDDataFileFU.PostedFile.ContentLength);
            }

            string tempFilePath = TEMP_FOLDER_PATH + "/" + "EC_TMP_" + fileName;
            tempFilePath = Server.MapPath(tempFilePath);

            MemoryStream byteStream = new MemoryStream(fileData);
            byteStream.Seek(0, SeekOrigin.Begin);
            using (FileStream fileStream = File.Create(tempFilePath))
            {
                byteStream.CopyTo(fileStream);
            }
            
            string excelConnString = null;
            if (fileExtension == ".xls")
                excelConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            else if (fileExtension == ".xlsx")
                excelConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

            excelConnString = String.Format(excelConnString, tempFilePath);
            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                excelConnection.Open();
                //-------------------This block of code checks if the excel file has 1 sheet with name "Sheet1" or not-------------
                DataTable dt = new DataTable();
                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    showPopup("No data found in excel sheet!");
                    return;
                }
                String[] excelSheets = new String[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                    excelSheets[i] = dt.Rows[i]["TABLE_NAME"].ToString();

                if (excelSheets.Length > 1 || excelSheets[0] != "Sheet1$")
                {
                    showPopup("Excel file must only have 1 sheet with name `Sheet1`");
                    return;
                }
                //-----------------------------------------------------------------------------------------------------------------
                //---------------------------------Main processing-----------------------------------------------------------------
                //string query = "SELECT *, " + ID + " as [EventID]," + certificateType + "[CertificateType], 0 as EventCategoryID from [Sheet1$]";
                string query = "SELECT * from [Sheet1$]";
                OleDbCommand command = new OleDbCommand(query, excelConnection);
                DbDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    string regNo = dr["Reg No"].ToString();
                    string studentName = dr["Student Name"].ToString();
                    string fatherName = dr["Father Name"].ToString();
                    string husbandName = dr["Husband Name"].ToString();
                    string programName = dr["Program Name"].ToString();
                    string school = dr["College/School"].ToString();
                    string position = dr["Position"].ToString();
                    string isLPUStudent = dr["Is LPU Student"].ToString();
                    string certType = dr["CertificateType"].ToString();

                    if (studentName.Trim().Length <= 2 || fatherName.Trim().Length <= 2 || certType.Trim().Length <= 1 || school.Trim().Length <= 1 || position.Trim().Length == 0 || isLPUStudent.Trim().Length == 0)
                    {
                        refreshMode = 4;
                        dr.Dispose();
                        command.Dispose();
                        dt.Dispose();
                        Response.Redirect(Request.RawUrl);
                        return;
                    }
                }

                dr.Dispose();
                command.Dispose();
                dt.Dispose();
            }

            File.Delete(tempFilePath);

            string result = ftpClient.UploadFile(folderOnFTPServer, fileName, fileData, credentials);
            if (result.Trim().StartsWith("226") || result.Trim().Contains("Success") || result.Trim().Contains("complete"))
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
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("UPDATE {0} SET ExcelName = @EN WHERE ID = @ID AND EventStatus = 2", "EventMaster");
                cmd.Connection = connection;
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
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while uploading the file.");
        }

    }
    private void showPopup(string text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

}
