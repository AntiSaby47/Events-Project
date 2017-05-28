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
using Microsoft.ApplicationBlocks.Data;

public partial class frmRegisterEventDSA : System.Web.UI.Page
{
    private static int refreshMode = 0;
    string connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    System.Data.DataSet dataset = new System.Data.DataSet();
    private string folderOnFTPServer = "test";
    private string TEMP_FOLDER_PATH = "~/Temp/";
    private static readonly string[] certificateTypes = { "CE", "CM", "CP", "CR", "CT" };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (refreshMode == 1)
        {
            showPopup("Event updated!");
            refreshMode = 0;
        }
        else if (refreshMode == 2)
        {
            showPopup("Data updated!");
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

        if (eventName.Length < 4)
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

                //connection.Open();
                //SqlCommand cmd = new SqlCommand();
                //cmd.CommandText = string.Format("SELECT ID FROM {0} WHERE EventName=@EN AND StartDate=@SD AND EndDate=@ED AND OrganisedBy=@OB;", "EventMaster");
                //cmd.Connection = connection;
                //cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                //cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                //cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                //cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                //int id = (int)cmd.ExecuteScalar();
                //cmd.Dispose();
                //return id;
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
        if (startDate >= DateTime.Now)
        {
            if (endDate >= DateTime.Now)
            {
                if (startDate <= endDate)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string parentID = null;
                            int es = 2;
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
                            param[6] = new SqlParameter("@hasCategories", allowSubEvents);

                            dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pRegisterEventEventCertificates]", param);
                            int rowsAffected = dataset.Tables[1].Rows.Count;


                            //SqlCommand cmd = new SqlCommand();
                            //cmd.CommandText = string.Format("INSERT INTO {0}({1}, {2}, {3}, {4}, {5}, {6}) VALUES(@EN, @SD, @ED, @AT, @OB, @aSE);", "EventMaster", "EventName", "StartDate", "EndDate", "EventStatus", "OrganisedBy", "HasCategories");
                            //cmd.Connection = connection;
                            //cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                            //cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                            //cmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = endDate;
                            //cmd.Parameters.Add("@AT", SqlDbType.SmallInt).Value = isActive;
                            //cmd.Parameters.Add("@OB", SqlDbType.VarChar).Value = organizedBy;
                            //cmd.Parameters.Add("@aSE", allowSubEvents);
                            //connection.Open();
                            //int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected <= 0)
                            {
                                showPopup("Couldn\\'t register event. Please reopen the page and try again.");
                            }

                            else
                            {
                                showPopup("Event registered. Note down the Event name and start date. You\\'ll need it while uploading event data.");
                            }
                            //cmd.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Ex: " + ex.Message);
                        showPopup("Something went wrong while registering the event!");
                        return;
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

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@status", status);
                    param[1] = new SqlParameter("@id", ID);
                    SqlCommand command = new SqlCommand("pUpdateEventStatusEventCertificates", connection);
                    command.Parameters.AddRange(param);
                    command.CommandType = CommandType.StoredProcedure;
                    int rowsAffected = command.ExecuteNonQuery();

                    //command.CommandText = string.Format("UPDATE {0} SET {1}=@ST WHERE {2}=@ID AND {3}=@PID", "EventMaster", "EventStatus", "id", "ParentEventID");
                    //command.Connection = connection;
                    //command.Parameters.Add("@ST", SqlDbType.Int).Value = status;
                    //command.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    //command.Parameters.Add("@PID", SqlDbType.Int).Value = pID;
                    //connection.Open();
                    //int rowsAffected = command.ExecuteNonQuery();
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

        else if (e.CommandName == "ViewExcel")
        {
            try
            {
                string ID;
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;

                //string query = string.Format("SELECT {0} FROM {1} WHERE {2} = @ID", "ExcelName", "EventMaster", "ID");

                DataTable data = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@organisedBy", null);
                    param[1] = new SqlParameter("@ID", ID);
                    dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pSchoolwiseDataEventCertificates]", param);
                    data = dataset.Tables[2];


                    //SqlCommand selectCommand = new SqlCommand(query, connection);
                    //selectCommand.Parameters.AddWithValue("@ID", ID);
                    //SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                    //connection.Open();
                    //adapter.Fill(data);
                    //selectCommand.Dispose();
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
                string ID, excelName; //, certificateType;
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;
                excelName = item["ExcelName"].Text;
                //certificateType = excelName[excelName.IndexOf("_") + 1].ToString();
                //Debug.WriteLine("Certificate Type: " + certificateType);

                //if (!certificateTypes.Contains(certificateType))
                //{
                //    showPopup("Something went wrong while verifying certificate type. Please check the excel data sheet.");
                //    return;
                //}

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
                        showPopup("Excel file must only have 1 sheet with the name of `Sheet1`");
                        return;
                    }
                    //-----------------------------------------------------------------------------------------------------------------
                    //---------------------------------Main processing-----------------------------------------------------------------
                    //string query = "SELECT *, " + ID + " as [EventID]," + certificateType + "[CertificateType], 0 as EventCategoryID from [Sheet1$]";
                    string query = "SELECT *, @ID as [EventID], 0 as [EventCategoryID] from [Sheet1$]";
                    OleDbCommand command = new OleDbCommand(query, excelConnection);
                    command.Parameters.Add("@ID", OleDbType.Integer).Value = ID;
                    //command.Parameters.Add("@CT", OleDbType.VarChar).Value = certificateType;

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
                        bulkInsert.ColumnMappings.Add("CertificateType", "CertificateType");
                        bulkInsert.WriteToServer(dr);
                    }

                    dr.Dispose();
                    command.Dispose();
                    dt.Dispose();
                }

                //string query2 = string.Format("UPDATE {0} SET {1} = 4 WHERE {2} = @ID AND {1} = 3", "EventMaster", "EventStatus", "id");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int status = 4;
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@status", status);
                    param[1] = new SqlParameter("@id", ID);
                    SqlCommand command = new SqlCommand("pUpdateEventStatusEventCertificates", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(param);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    //SqlCommand sqlCommand = new SqlCommand(query2, connection);
                    //sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    //connection.Open();
                    //int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                        showPopup("Something went wrong while approving the file. Please try again later.");
                    else
                    {
                        refreshMode = 2;
                        Response.Redirect(Request.RawUrl);
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Ex: " + ex.Message);
                showPopup("Something went wrong while approving the file. Please try again later. [1]");
                return;
            }

        }

        else if (e.CommandName == "RejectExcel")
        {
            GridDataItem item = e.Item as GridDataItem;
            string ID = item["id"].Text;
            string query2 = string.Format("UPDATE {0} SET {1} = 'REJECTED', {2} = 2 WHERE {3} = @ID", "EventMaster", "ExcelName", "EventStatus", "id");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(query2, connection);
                sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                connection.Open();
                int rowsAffected = sqlCommand.ExecuteNonQuery();
                if (rowsAffected <= 0)
                    showPopup("Something went wrong while approving the file. Please try again later.");
                else
                {
                    refreshMode = 2;
                    Response.Redirect(Request.RawUrl);
                }
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
            if (eventStatus != 1)
            {
                RadButton approveBtn = item.FindControl("redSEReqAllowBtn") as RadButton;
                RadButton rejectBtn = item.FindControl("redSEReqRejectBtn") as RadButton;
                approveBtn.Visible = rejectBtn.Visible = false;
            }

            if (eventStatus != 3)
            {
                RadButton viewBtn = item.FindControl("redExcelBtn") as RadButton;
                RadButton approveExcelBtn = item.FindControl("redApproveExcelBtn") as RadButton;
                RadButton rejectExcelBtn = item.FindControl("redRejectExcelBtn") as RadButton;
                viewBtn.Visible = approveExcelBtn.Visible = rejectExcelBtn.Visible = false;
            }
        }
    }
    private void showPopup(string text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

}