using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data;

public partial class TestReportFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Certificate.rdl");

            string conString = ConfigurationManager.ConnectionStrings["NewUmsConnectionString"].ConnectionString;

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
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(datasource);
        }
    }
}