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
using Microsoft.Reporting.WebForms;
using Microsoft.ApplicationBlocks.Data;
using Ionic.Zip;

public partial class frmEventsManage_PrintCerts : System.Web.UI.Page
{
    private static int refreshMode = 0;
    String connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    System.Data.DataSet dataset = new System.Data.DataSet();
    private string folderOnFTPServer = "test";
    private static string mode = null;

    enum CertTypes : int
    {
        CP,
        CE,
        CM,
        CR,
        CT
    };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            loadSchools(MEPCEditLoadSchoolsCB);
        }
        if (refreshMode == 1)
        {
            showPopup("Data updated!");
            refreshMode = 0;
        }
    }

    protected void repPrintCertsRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "ViewExcel")
        {
            try
            {
                string ID;
                GridDataItem item = e.Item as GridDataItem;
                ID = item["id"].Text;

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

        if (e.CommandName == "DownloadPDF")
        {
            string ID;
            GridDataItem item = e.Item as GridDataItem;
            ID = item["id"].Text;
            CreatePDF(ID);
        }

        else if (e.CommandName == "MarkPrinted")
        {
            int ID;
            GridDataItem item = e.Item as GridDataItem;
            ID = Int32.Parse(item["id"].Text);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int status = 5;
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@status", status);
                param[1] = new SqlParameter("@id", ID);
                SqlCommand command = new SqlCommand("pUpdateEventStatusEventCertificates", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected <= 0)
                {
                    Response.Write("<script>alert('Something went wrong while updating data!');</script>");
                }
                else
                {
                    refreshMode = 1;
                    Response.Redirect(Request.RawUrl);
                }
            }
        }
    }

    private void CreatePDF(string ID)
    {
        try
        {
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            // Setup the report viewer object and get the array of bytes
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = Server.MapPath("~/AllCerts.rdl");

            string conString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@eid", ID);
                //param[1] = new SqlParameter("@certificateType", temp.Rows[0]["CertificateType"]);
                dataset = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "[pEventCertificateData_WO]", param);
                table = dataset.Tables[0];
            }

            DataTable[] tableArray = new DataTable[5];
            for (int i = 0; i < 5; i++)
                tableArray[i] = table.Clone();

            foreach(DataRow row in table.Rows)
            {
                if (((string)row["certificateType"]) == "CP")
                    tableArray[(int)CertTypes.CP].ImportRow(row);
                else if (((string)row["certificateType"]) == "CR")
                    tableArray[(int)CertTypes.CR].ImportRow(row);
                else if (((string)row["certificateType"]) == "CE")
                    tableArray[(int)CertTypes.CE].ImportRow(row);
                else if (((string)row["certificateType"]) == "CM")
                    tableArray[(int)CertTypes.CM].ImportRow(row);
                else if (((string)row["certificateType"]) == "CT")
                    tableArray[(int)CertTypes.CT].ImportRow(row);
            }

            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/zip";
            Response.AppendHeader("content-disposition", "attachment; filename=Reports.zip");

            using (ZipFile zipFile = new ZipFile())
            {
                foreach (DataTable tempTable in tableArray)
                {
                    if (tempTable.Rows.Count != 0)
                    {
                        string certType = (string)tempTable.Rows[0]["certificateType"];
                        string eid = ID;
                        ReportParameter[] parms = new ReportParameter[2];
                        parms[0] = new ReportParameter("eid", eid);
                        parms[1] = new ReportParameter("certificateType", certType);

                        ReportDataSource datasource = new ReportDataSource("DataSet1", tempTable);
                        viewer.LocalReport.DataSources.Clear();
                        viewer.LocalReport.DataSources.Add(datasource);
                        viewer.LocalReport.SetParameters(parms);

                        byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                        MemoryStream byteStream = new MemoryStream(bytes);
                        byteStream.Seek(0, SeekOrigin.Begin);

                        string fileName = certType + ".pdf";
                        zipFile.AddEntry(fileName, byteStream);
                        //byteStream.Dispose();
                    }
                }
                zipFile.Save(Response.OutputStream);
            }
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while generating the PDF!");
        }
    }

    protected void StudentType_CheckedChanged(object sender, EventArgs e)
    {
        if (repLPUStudentRB.Checked)
        {
            rowRegNo.Visible = true;
            rowCertNo.Visible = false;
            rowButton.Visible = true;
            mode = "LPUStudent";
        }
        else
        {
            rowCertNo.Visible = true;
            rowRegNo.Visible = false;
            rowButton.Visible = true;
            mode = "Outsider";
        }
    }
    protected void repFindCertBtn_Click(object sender, EventArgs e)
    {
        try
        {
            string regNo = repRegNoTB.Text.Trim();
            string certNo = repCertNoTB.Text.Trim();
            string query = "";
            if (mode == "LPUStudent")
            {
                query = "SELECT ec.StudentName[StudentName], em2.EventName[ParentEventName], em1.EventName[EventName], Position[Position], CAST(em1.StartDate AS DATE)[StartDate], CAST(em1.EndDate AS DATE)[EndDate], em1.OrganisedBy[OrganisedBy], ec.CertificateType[CertificateType] FROM EventMaster em1 INNER JOIN EventMaster em2 ON em1.ParentEventID = em2.id INNER JOIN EventCertificates ec ON ec.EventID = em1.id WHERE em1.ParentEventID IS NOT NULL AND ec.RegisterationNumber = @RegNo" +
                        " UNION " +
                        "SELECT ec.StudentName[StudentName], 'N.A.'[ParentEventName], em1.EventName[EventName], Position[Position], CAST(em1.StartDate AS DATE)[StartDate], CAST(em1.EndDate AS DATE)[EndDate], em1.OrganisedBy[OrganisedBy], ec.CertificateType[CertificateType] FROM EventMaster em1 INNER JOIN EventCertificates ec ON ec.EventID = em1.id WHERE ec.RegisterationNumber = @RegNo AND em1.ParentEventID IS NULL";
            }
            else
            {
                query = "SELECT ec.StudentName[StudentName], em2.EventName[ParentEventName], em1.EventName[EventName], Position[Position], CAST(em1.StartDate AS DATE)[StartDate], CAST(em1.EndDate AS DATE)[EndDate], em1.OrganisedBy[OrganisedBy], ec.CertificateType[CertificateType] FROM EventMaster em1 INNER JOIN EventMaster em2 ON em1.ParentEventID = em2.id INNER JOIN EventCertificates ec ON ec.EventID = em1.id WHERE em1.ParentEventID IS NOT NULL AND ec.CertificateNumber = @CertNo" +
                        " UNION " +
                        "SELECT ec.StudentName[StudentName], 'N.A.'[ParentEventName], em1.EventName[EventName], Position[Position], CAST(em1.StartDate AS DATE)[StartDate], CAST(em1.EndDate AS DATE)[EndDate], em1.OrganisedBy[OrganisedBy], ec.CertificateType[CertificateType] FROM EventMaster em1 INNER JOIN EventCertificates ec ON ec.EventID = em1.id WHERE ec.CertificateNumber = @CertNo AND em1.ParentEventID IS NULL";
            }
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand(query, connection);
                if (mode == "LPUStudent")
                    selectCommand.Parameters.Add("@Regno", SqlDbType.VarChar).Value = regNo;
                else
                    selectCommand.Parameters.Add("@CertNo", SqlDbType.VarChar).Value = certNo;
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(table);
                repCertInfoRG.DataSource = table;
                repCertInfoRG.DataBind();
                repCertInfoRG.Visible = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading data!");
        }
    }
    protected void repCertInfoRG_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "DownloadSample")
        {
            try
            {
                GridDataItem item = e.Item as GridDataItem;
                string certType = item["CertificateType"].Text;
                string fileName = "Events_Certificate_Format_" + certType + ".docx";
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
    }
    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
    private void loadSchools(RadComboBox combobox)
    {
        combobox.Items.Clear();
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
    private void loadEvents(RadComboBox combobox)
    {
        combobox.Items.Clear(); //Clear the Combo items to make sure duplicates are not made
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@organisedBy", MEPCEditLoadSchoolsCB.SelectedValue);
                param[1] = new SqlParameter("@ID", null);
                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pSchoolwiseDataEventCertificates]", param);

                DataTable types = new DataTable();
                types = dataset.Tables[4];
                List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(types.Rows.Count);

                foreach (DataRow row in types.Rows)
                {
                    RadComboBoxItem itemData = new RadComboBoxItem();
                    itemData.Value = row["id"].ToString();
                    string sqlFormattedstartDate = ((DateTime)row["StartDate"]).ToString("dd-MM-yyyy");
                    string sqlFormattedEndDate = ((DateTime)row["EndDate"]).ToString("dd-MM-yyyy");
                    string text = row["EventName"] + " :: " + sqlFormattedstartDate + " to " + sqlFormattedEndDate;
                    itemData.Text = text;
                    combobox.Items.Add(itemData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading events!");
        }
    }
    protected void MEPCEditLoadSchoolsCB_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        loadEvents(MEPCEditLoadEventsCB);
    }

    protected void MEPCEditSearchBtn_Click(object sender, EventArgs e)
    {
        Debug.WriteLine("AAAAAAAAAAAAAAAAAAAA" + MEPCEditLoadEventsCB.SelectedValue);
        string eid = MEPCEditLoadEventsCB.SelectedValue;
        try
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@eid", eid);
                dataset = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "[pEventCertificateData]", param);
                Debug.WriteLine("TOTAL: " + dataset.Tables[0].Columns.Count);
                MEPCEditStudentDataRG.DataSource = dataset.Tables[0];
                MEPCEditStudentDataRG.Visible = true;
                MEPCEditStudentDataRG.Rebind();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while loading data!");
        }
    }
    protected void MEPCEditStudentDataRG_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "EditData")
        {
            PopUp.Show();
        }
    }
}