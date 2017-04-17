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
using System.IO;
using System.Data.OleDb;
using System.Data.Common;

public partial class frmEventsManage_ManageEvent : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    private string TEMP_FOLDER_PATH = "~/Temp/";
    private static int selectedEventId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            selectedEventId = -1;
        }
    }
    protected void meSubmitDateBTN_Click(object sender, EventArgs e)
    {
        DateTime startDate = new DateTime();

        try
        {
            startDate = (DateTime) meStartDateDP.SelectedDate;
        }

        catch(InvalidOperationException ex)
        {
            showPopup("Select a valid date.");
        }

        String eventName = meEventNameTB.Text;

        //-----------------Validate input---------------------------------
        if (eventName.Length <= 4)
        {
            showPopup("Please check the input.");
            return;
        }

        int id = checkIfEventExists(eventName, startDate);
        if(id > 0)
        {
            selectedEventId = id;
            uploadTable.Style.Remove("display");
        }
        else
        {
            showPopup("No Event found with said details!");
            uploadTable.Style.Add("display","none");
            selectedEventId = -1;
        }
    }

    protected void meUploadExcelBTN_Click(object sender, EventArgs e)
    {
        if (selectedEventId > 0)
            uploadExcelToDB();
        else
            Response.Redirect(Request.RawUrl);
    }

    //===================================Database Operations=======================================

    private int checkIfEventExists(String eventName, DateTime startDate)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT ID FROM EventMaster WHERE EventName = @EN AND StartDate = @SD", connection);
                cmd.Parameters.Add("@EN", SqlDbType.VarChar).Value = eventName;
                cmd.Parameters.Add("@SD", SqlDbType.DateTime).Value = startDate;
                int id = (int)cmd.ExecuteScalar();
                cmd.Dispose();
                return id;
            }
            catch(NullReferenceException)
            {
                return -1;
            }
        }
    }

    //===================================Utilities================================================
    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }

    //===================================Excel Utilities==========================================

    private void uploadExcelToDB()
    {
        string fileName = meExcelFU.ResolveClientUrl(meExcelFU.PostedFile.FileName);
        string fileExtension = Path.GetExtension(fileName);
        string tempFileSavePath = Server.MapPath(TEMP_FOLDER_PATH) + fileName;

        if(fileExtension != ".xls" && fileExtension != ".xlsx")
        {
            showPopup("Please upload a valid excel file.");
            return;
        }

        meExcelFU.SaveAs(tempFileSavePath); //Save file on server

        //------------------Connection strings--------------------------------------
        
        string excelConnString = null;

        if (fileExtension == ".xls")
            excelConnString = ConfigurationManager.ConnectionStrings["xlsConnString"].ConnectionString;
        else if (fileExtension == ".xlsx")
            excelConnString = ConfigurationManager.ConnectionStrings["xlsxConnString"].ConnectionString;

        excelConnString = String.Format(excelConnString, tempFileSavePath);

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
            string query = "SELECT *, " + selectedEventId + " as [EventID], 0 as EventCategoryID from [Sheet1$]";
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
                bulkInsert.ColumnMappings.Add("EventCategoryID", "EventCategoryID");
                bulkInsert.WriteToServer(dr);
            }

            dr.Dispose();
            command.Dispose();
            dt.Dispose();
        }

    }

}