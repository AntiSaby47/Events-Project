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
        if(refreshMode == 1)
        {
            RadTabStrip1.Tabs[1].Selected = true;
            RadTabStrip1.SelectedIndex = 1;
            RadPageView2.Selected = true;
            refreshMode = 0;
        }

        if (links.Count > 0)
        {
            int i = 0;
            foreach (string item in links)
            {
                Button btn = new Button();
                btn.ID = i + "";
                btn.Text = "Download";
                btn.Click += new EventHandler(downloadFile);
                TableRow row = new TableRow();
                TableCell cellName = new TableCell();
                cellName.Text = item;
                row.Cells.Add(cellName);
                TableCell cellButton = new TableCell();
                cellButton.Controls.Add(btn);
                row.Cells.Add(cellButton);
                USDFilesTbl.Rows.Add(row);
                i++;
            }
        }
    }
    protected void USCUploadBtn_Click(object sender, EventArgs e)
    {
        try
        {
            FtpService ftpClient = new FtpService();
            FtpService.FtpCredentials credentials = FtpUserPassword.GetUMSFtpCredentials();

            regno = USDRegNotxt.Text;
            ft = USDCertTypeCombo.SelectedValue;
            string fileType = null;
            if (ft == "A") { fileType = "AdmissionDocument"; } else if (ft == "R") { fileType = "AcademicCertificate"; } else if (ft == "E") { fileType = "EventCertificate"; }
            int i = 1;
            ArrayList filesToUpload = new ArrayList();
            foreach (HttpPostedFile postedFile in USDFileUpload.PostedFiles)
            {
                string ext = Path.GetExtension(postedFile.FileName).ToLower();
                string time = DateTime.Now.ToString("HHmmss");
                string fileName = regno + "_" + fileType + "_" + i + "_" + time + ext;

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
                    Debug.WriteLine("Uploaded File: " + i);
                    i += 1;
                    filesToUpload.Add(fileName);
                }
                else if (result.Trim() == "File Already Exists")
                {
                    showPopup("A certificate file already exists with the same name. Please change the name of the image file and try again.");
                    break;
                }
                else
                {
                    Debug.WriteLine(result);
                    showPopup("Something went wrong. Please try again.");
                    break;
                }
            }

            if(USDFileUpload.PostedFiles.Count == filesToUpload.Count)
            {
                showPopup("Uploaded!");
            }

            else
            {
                foreach(string fileName in filesToUpload)
                {
                    string result = ftpClient.DeleteFile(folderOnFTPServer, fileName, credentials);
                    Debug.WriteLine("Deletion status: " + result);
                }
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
                    string splitPoint = folderOnFTPServer + "/";
                    string splitFilename = item.Substring(item.IndexOf(splitPoint) + splitPoint.Length);
                    links.Add(splitFilename);
                }
            }

            if (links.Count > 0)
            {
                refreshMode = 1;
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                showPopup("No files could be retrieved.");
            }
        }

        catch(Exception ex)
        {
            Debug.WriteLine("Exception ::" + ex.Message);
        }
    }

    protected void downloadFile(object sender, EventArgs e)
    {
        try
        {
            Button btn = (Button)sender;
            int id = Int32.Parse(btn.ID);
            Debug.WriteLine(id);
            string fileName = USDFilesTbl.Rows[id + 1].Cells[0].Text;
            Debug.WriteLine("Filename: " + fileName);

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
        catch(Exception ex)
        {
            refreshMode = 0;
            showPopup("Something went wrong while downloading the file!");
            Debug.WriteLine(ex.Message);
        }
    }


    private void showPopup(String text)
    {
        Response.Write("<script>alert(' " + text + "');</script>");
    }
}