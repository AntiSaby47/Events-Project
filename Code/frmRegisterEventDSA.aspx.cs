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
using FtpClient;
using System.Net;
using System.IO;
using System.Data.OleDb;
using System.Data.Common;

public partial class frmRegisterEventDSA : System.Web.UI.Page
{
    private static int refreshMode = 0;
    string connectionString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;
    private string folderOnFTPServer = "test";
    private string TEMP_FOLDER_PATH = "~/Temp/";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (refreshMode == 1)
        {
            showPopup("Event updated!");
            refreshMode = 0;
        }
    }

    protected void resERegisterBtn_Click(object sender, EventArgs e)
    {
        string eventName = redENametxt.Text.Trim();
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        string organizedBy = redEOrganizedBy.Text.Trim();
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
    private int checkIfAlreadyRegistered(string eventName, DateTime startDate, DateTime endDate, string organizedBy)
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
    private void registerEvent(string eventName, DateTime startDate, DateTime endDate, string organizedBy, int allowSubEvents)
    {
        int isActive = 2;
        try 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO EventMaster(EventName, StartDate, EndDate, EventStatus, OrganisedBy, HasCategories) VALUES(@EN, @SD, @ED, @AT, @OB, @aSE)", connection);
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

    private void showPopup(string text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

    protected void redSERegRequestRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "Approve" || e.CommandName == "Reject")
        {
            try
            {
                int ID, pID;
                GridDataItem item = e.Item as GridDataItem;
                ID = Int32.Parse(item["id"].Text);
                pID = Int32.Parse(item["parentEventID"].Text);

                string query = null;
                int status = 0;
                if (e.CommandName == "Approve")
                    status = 2;
                if (e.CommandName == "Reject")
                    status = -1;

                query = "UPDATE dbo.EventMaster SET EventStatus=@ST WHERE id=@ID AND ParentEventID=@PID";
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
                        refreshMode = 1;
                        Response.Redirect(Request.RawUrl);
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

        else if(e.CommandName == "ViewExcel")
        {
            try
            {
                string ID, pID;
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;

                string query = "SELECT ExcelName FROM EventMaster " +
                "WHERE ID = @ID";

                DataTable data = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand selectCommand = new SqlCommand(query, connection);
                    selectCommand.Parameters.AddWithValue("@ID", ID);
                    SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                    connection.Open();
                    adapter.Fill(data);
                    selectCommand.Dispose();
                }

                string fileName = "";
                if (data.Rows.Count == 1)
                {
                    if (data.Rows[0]["ExcelName"] != System.DBNull.Value)
                        fileName = (string)data.Rows[0]["ExcelName"];
                    else
                    {
                        showPopup("Something went wrong while fetching the file. Please try again.[1]");
                        return;
                    }
                }

                else
                {
                    showPopup("Something went wrong while fetching the file. Please try again.[2]");
                    return;
                }

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
                Debug.WriteLine("Exception: " + ex.Message);
                showPopup("Something went wrong while fetching the file. Please try again [3].");
            }
        }

        else if (e.CommandName == "ApproveExcel")
        {
            try
            {
                string ID, excelName;
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;
                excelName = item["ExcelName"].Text;
                string fileExtension = Path.GetExtension(excelName);
                FtpService ftpClient = new FtpService();
                FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();
                FtpWebResponse response = ftpClient.DowloadFile(folderOnFTPServer, excelName, FtpUserPassword.GetUMSFtpCredentials());

                string tempFilePath = TEMP_FOLDER_PATH + "/" + "TMP_" + excelName;
                tempFilePath = Server.MapPath(tempFilePath);
                using (FileStream fileStream = File.Create(tempFilePath))
                {
                    response.GetResponseStream().CopyTo(fileStream);
                }

                string excelConnString = null;
                if (fileExtension == ".xls")
                    excelConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                else if (fileExtension == ".xlsx")
                    excelConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                excelConnString = String.Format(excelConnString, tempFilePath);
                Debug.WriteLine("ExcelString: " + excelConnString);

                using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
                {
                    Debug.WriteLine("CP1");
                    excelConnection.Open();
                    //-------------------This block of code checks if the excel file has 1 sheet with name "Sheet1" or not-------------
                    Debug.WriteLine("CP2");
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
                        showPopup("Excel file must only have 1 sheet with the name of `Sheet1`");
                        return;
                    }
                    //-----------------------------------------------------------------------------------------------------------------
                    //---------------------------------Main processing-----------------------------------------------------------------
                    string query = "SELECT *, " + ID + " as [EventID], 0 as EventCategoryID from [Sheet1$]";
                    OleDbCommand command = new OleDbCommand(query, excelConnection);
                    DbDataReader dr = command.ExecuteReader();

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlBulkCopy bulkInsert = new SqlBulkCopy(connection);
                        bulkInsert.DestinationTableName = "EventCertificates";
                        bulkInsert.ColumnMappings.Add("Reg No", "RegisterationNumber"); // Source(Excel column), Destination(Database Table Column)
                        bulkInsert.ColumnMappings.Add("Student Name", "StudentName");
                        bulkInsert.ColumnMappings.Add("Father Name", "FatherName");
                        bulkInsert.ColumnMappings.Add("Husband Name", "HusbandName");
                        bulkInsert.ColumnMappings.Add("Program Name", "ProgramName");
                        bulkInsert.ColumnMappings.Add("College/School", "CollegeOrSchool");
                        bulkInsert.ColumnMappings.Add("Position", "Position");
                        bulkInsert.ColumnMappings.Add("Is LPU Student", "IsLPUStudent");
                        bulkInsert.ColumnMappings.Add("EventID", "EventID");
                        bulkInsert.WriteToServer(dr);
                    }

                    dr.Dispose();
                    command.Dispose();
                    dt.Dispose();
                }

                string query2 = "UPDATE EventMaster SET EventStatus = 3 WHERE id = @ID AND EventStatus = 2";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand(query2, connection);
                    sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    connection.Open();
                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                        showPopup("Something went wrong while approving the file. Please try again later.");
                    else
                        Response.Redirect(Request.RawUrl);
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Ex: " + ex.Message);
                showPopup("Something went wrong while approving the file. Please try again later. [1]");
                return;
            }

        }
    }

    protected void redSERegRequestRG_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = e.Item as GridDataItem;
            int eventStatus = Int32.Parse(item["EventStatus"].Text);
            string excelName = item["ExcelName"].Text;
            Debug.WriteLine("Name: " + excelName);
            if(eventStatus != 0)
            {
                RadButton approveBtn = item.FindControl("redSEReqAllowBtn") as RadButton;
                RadButton rejectBtn = item.FindControl("redSEReqRejectBtn") as RadButton;
                approveBtn.Visible = rejectBtn.Visible = false;
            }

            if (eventStatus != 2 || excelName == "&nbsp;")
            {
                RadButton viewBtn = item.FindControl("redExcelBtn") as RadButton;
                RadButton approveExcelBtn = item.FindControl("redApproveExcelBtn") as RadButton;
                viewBtn.Visible = approveExcelBtn.Visible = false;
            }
        }
    }
}