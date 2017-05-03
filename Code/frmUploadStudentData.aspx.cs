using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.IO;
using System.Net;
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using FtpClient;
using System.Collections;

public partial class frmUploadStudentData : System.Web.UI.Page
{
    private static string regno;
    private static String ft;
    private static int refreshMode = 0;
    private static string folderOnFTPServer = "test";
    private String connectionString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;
    private static ArrayList links = new ArrayList();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (links.Count > 0)
        {
            int i = 0;
            foreach (string item in links)
            {
                Button btn = new Button();
                btn.ID = ++i + "Nigga";
                btn.Text = "Download";
                btn.Click += new EventHandler(downloadFile);
                TableRow row = new TableRow();
                TableCell cellName = new TableCell();
                cellName.Text = "Nigga";
                row.Cells.Add(cellName);
                TableCell cellButton = new TableCell();
                cellButton.Controls.Add(btn);
                row.Cells.Add(cellButton);
                USDFilesTbl.Rows.Add(row);
            }
        }
    }
    protected void USCUploadBtn_Click(object sender, EventArgs e)
    {
        try
        {
            regno = USDRegNotxt.Text;
            ft = USDCertTypeCombo.SelectedValue;
            string fileType = null;
            if (ft == "A") { fileType = "AdmissionDocument"; } else if (ft == "R") { fileType = "AcademicCertificate"; } else if (ft == "E") { fileType = "EventCertificate"; }
            int i = 1;
            foreach (HttpPostedFile postedFile in USDFileUpload.PostedFiles)
            {

                string ext = Path.GetExtension(postedFile.FileName).ToLower(); 

                string fileName = regno + "_" + fileType + "_" + i + ext;

                FtpService ftpClient = new FtpService();
                FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();

                byte[] imageData = null;
                if (!string.IsNullOrEmpty(fileName))
                {
                    using (BinaryReader binaryReader = new BinaryReader(postedFile.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(postedFile.ContentLength);
                    }
                }

                string fullPath = credentials.Url + folderOnFTPServer + "/" + fileName;

                string result = ftpClient.UploadFile(folderOnFTPServer, fileName, imageData, credentials);
                
                if (result.Trim().StartsWith("226") || result.Trim().Contains("complete") || result.Trim().Contains("Success"))
                {
                    showPopup("Uploaded!");
                    Debug.WriteLine("Uploaded!");
                }
                else if (result.Trim() == "File Already Exists")
                {
                    showPopup("A certificate file already exists with the same name. Please change the name of the image file and try again.");
                    return;
                }
                else
                {
                    Debug.WriteLine(result);
                    showPopup("Something went wrong. Please try again.");
                    return;
                }
                i += 1;
            }
        }
        catch (Exception ex)
        {
            showPopup("Something went wrong. Please try again.");
            Debug.WriteLine(ex.Message);
        }
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        try
        {
            regno = USDRegNotxt2.Text;
            ft = USDCertTypeCombo2.SelectedValue;
            string fileType = null;
            if (ft == "A") { fileType = "AdmissionDocument"; } else if (ft == "R") { fileType = "AcademicCertificate"; } else if (ft == "E") { fileType = "EventCertificate"; }

            ArrayList allFiles = new ArrayList();
            links.Clear();

            string fileName = regno + "_" + fileType;

            FtpService ftpClient = new FtpService();
            FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();
            allFiles.AddRange(ftpClient.ViewFile(folderOnFTPServer, credentials));
            foreach (string item in allFiles)
            {
                if(item.Contains(fileName))
                {
                    links.Add(item);
                }
            }

            if (links.Count > 0)
            {
                Response.Redirect(Request.RawUrl);
            }
            else
                showPopup("No files could be retrieved.");
        }

        catch(Exception ex)
        {
            Debug.WriteLine("Exception ::" + ex.Message);
        }
    }

    protected void downloadFile(object sender, EventArgs e)
    {
        Debug.WriteLine("Test");
        //Button btn = (Button)sender;
        //Debug.WriteLine("ID ::" + btn.ID);
    }


    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}