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

public partial class frmEventsManage_PrintCerts : System.Web.UI.Page
{
    private static int refreshMode = 0;
    String connectionString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;
    private string folderOnFTPServer = "test";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (refreshMode == 1)
        {
            showPopup("Data updated!");
            refreshMode = 0;
        }
    }

    protected void repPrintCertsRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "DownloadPDF")
        {
            CreatePDF("report");
        }

        else if (e.CommandName == "MarkPrinted")
        {
            int ID, pID;
            GridDataItem item = e.Item as GridDataItem;
            ID = Int32.Parse(item["id"].Text);
            string query = "UPDATE EventMaster SET EventStatus = 4 WHERE id=@ID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
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

    private void CreatePDF(string fileName)
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
            viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Certificate.rdl");

            string conString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;

            SqlCommand cmd = new SqlCommand("SELECT * from EventCertificates");
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(table);
                }
            }

            System.Diagnostics.Debug.WriteLine("Size: " + table.Rows.Count);
            ReportDataSource datasource = new ReportDataSource("NewEventsDataSet", table);
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(datasource);

            byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + extension);
            Response.BinaryWrite(bytes); // create the file
            Response.Flush(); // send it to the client to download
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            showPopup("Something went wrong while generating the PDF!");
        }
    }



    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}