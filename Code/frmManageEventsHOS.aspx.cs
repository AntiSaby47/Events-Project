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
using System.Data.OleDb;
using System.Data.Common;

public partial class frmManageEventsHOS : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    private static string parentID;
    private static string mode, uploadDataMode;
    private string folderOnFTPServer = "test";
    private static int id = -47;
    private static int refreshMode = 0;
    private string TEMP_FOLDER_PATH = "~/Temp/";
    private static readonly string[] certificateTypes = { "E", "M", "P", "R", "T" };

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            loadSchools(rehSelectSchoolCB);
            loadSchools(rehSelectSchool2CB);
        }

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

    private void loadSchools(RadComboBox combobox)
    {
        combobox.Items.Clear();
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("SELECT Name,DivisionSectionCode FROM dbo.eGovWingMaster , eGovDivisionMaster WHERE eGovWingMaster.Type ='F' AND eGovWingMaster.Type= eGovDivisionMaster.WingType  AND  eGovDivisionMaster.IsActive =1 AND eGovWingMaster.Id = eGovDivisionMaster.WingId AND eGovWingMaster.IsActive =1 AND DivisionSectionCode IS NOT NULL ORDER BY 2 ", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                DataTable types = new DataTable();
                adapter.Fill(types);

                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);
                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["DivisionSectionCode"].ToString();
                    string text = row["DivisionSectionCode"] + " :: " + row["Name"];
                    itemData.Text = text;
                    combobox.Items.Add(itemData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading schools!");
        }
    }
    protected void rehSelectSchoolCB_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        try
        {
            string query = "(SELECT em1.EventName[EventName], em1.StartDate[StartDate], em1.id[id], em2.EventName[ParentEventName], em1.ParentEventID[ParentEventID] FROM EventMaster em1 INNER JOIN dbo.EventMaster em2 ON em1.parentEventId = em2.id WHERE em1.EventStatus = 0 AND em1.HasCategories IS NULL AND em1.organisedby=@school)" +
                            " UNION " +
                            "(SELECT EventName[EventName], StartDate[StartDate], id[id], 'N.A.'[ParentEventName], ParentEventID[ParentEventID] FROM EventMaster WHERE EventStatus = 0 AND HasCategories IS NULL AND ParentEventID IS NULL AND organisedby=@school)";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(query, connection);
                selectCommand.Parameters.Add("@school", SqlDbType.VarChar, 30).Value = rehSelectSchoolCB.SelectedValue;
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(table);
                rehApproveEventsRG.DataSource = table;
                rehApproveEventsRG.DataBind();
                rehApproveEventsRG.Visible = true;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading data!");
        }
    }


    private void showPopup(string text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

    protected void rehApproveEventsRG_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "ApproveEvent" || e.CommandName == "RejectEvent")
        {
            try
            {
                GridDataItem item = e.Item as GridDataItem;
                int ID = Int32.Parse(item["id"].Text);
                int status = 0;
                if (e.CommandName == "RejectEvent")
                    status = -1;
                else
                {
                    if (item["ParentEventName"].Text == "N.A.")
                        status = 2;
                    else
                        status = 1;
                }
                string query = null;
                query = "UPDATE EventMaster SET EventStatus=@ST WHERE id=@ID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@ST", SqlDbType.Int).Value = status;
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                    {
                        showPopup("Something went wrong while approving/rejecting the event.");
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
    }

    protected void rehSelectSchool2CB_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        try
        {
            string query = "(SELECT em1.EventName[EventName],em1.ExcelName[ExcelName], em1.StartDate[StartDate], em1.id[id], em2.EventName[ParentEventName], em1.ParentEventID[ParentEventID] FROM EventMaster em1 INNER JOIN dbo.EventMaster em2 ON em1.parentEventId = em2.id WHERE em1.EventStatus = 2 AND em1.ExcelName IS NOT NULL AND em1.HasCategories IS NULL AND em1.organisedby=@school)" +
                            " UNION " +
                            "(SELECT EventName[EventName],ExcelName[ExcelName], StartDate[StartDate], id[id], 'N.A.'[ParentEventName], ParentEventID[ParentEventID] FROM EventMaster WHERE EventStatus = 2 AND HasCategories IS NULL AND ParentEventID IS NULL AND ExcelName IS NOT NULL AND organisedby=@school)";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(query, connection);
                selectCommand.Parameters.Add("@school", SqlDbType.VarChar).Value = rehSelectSchool2CB.SelectedValue;
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(table);
                rehApproveExcelRG.DataSource = table;
                rehApproveExcelRG.DataBind();
                rehApproveExcelRG.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading data!");
        }
    }
    protected void rehApproveExcelRG_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "ViewData")
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

        else if (e.CommandName == "ApproveData")
        {
            try
            {
                string ID, parentName, certificateType, excelName;
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;
                excelName = item["ExcelName"].Text;
                parentName = item["ParentEventName"].Text;

                if (parentName == "N.A.")
                {
                    certificateType = excelName[excelName.IndexOf("_") + 1].ToString();
                    Debug.WriteLine("Certificate Type: " + certificateType);

                    if (!certificateTypes.Contains(certificateType))
                    {
                        showPopup("Something went wrong while verifying certificate type. Please check the excel data sheet.");
                        return;
                    }

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
                        string query = "SELECT *, @ID as [EventID], @CT as [CertificateType], 0 as [EventCategoryID] from [Sheet1$]";
                        OleDbCommand command = new OleDbCommand(query, excelConnection);
                        command.Parameters.Add("@ID", OleDbType.Integer).Value = ID;
                        command.Parameters.Add("@CT", OleDbType.VarChar).Value = certificateType;

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
                }

                int status = 0;
                if (item["ParentEventName"].Text == "N.A.")
                    status = 4;
                else
                    status = 3;

                string query2 = "UPDATE EventMaster SET EventStatus = @Status WHERE id = @ID AND EventStatus = 2";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand(query2, connection);
                    sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    sqlCommand.Parameters.Add("@Status", SqlDbType.Int).Value = status;
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

            catch (Exception ex)
            {
                Debug.WriteLine("Ex: " + ex.Message);
                showPopup("Something went wrong while approving the file. Please try again later. [1]");
                return;
            }

        }

        else if (e.CommandName == "RejectData")
        {
            GridDataItem item = e.Item as GridDataItem;
            string ID = item["id"].Text;
            string query2 = "UPDATE EventMaster SET ExcelName = 'REJECTED' WHERE id = @ID";
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
}