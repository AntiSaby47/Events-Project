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

public partial class frmEventsManage_PrintCerts : System.Web.UI.Page
{
    String connectionString = ConfigurationManager.ConnectionStrings["TestCS"].ConnectionString;
    private string folderOnFTPServer = "test";

    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void repPrintCertsRG_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "DownloadExcel")
        {
            try
            {
                String ID;
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
    }

    protected void repPrintCertsRG_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {


    }

    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}