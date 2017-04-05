using System;
using FtpClient;
using System.Web.Configuration;

public class FtpUserPassword
{
    public static FtpService.FtpCredentials GetUMSFtpCredentials()
    {
        FtpService.FtpCredentials credentials = new FtpService.FtpCredentials();
        credentials.username = WebConfigurationManager.AppSettings["FTPUmsUsername"];
        credentials.password = WebConfigurationManager.AppSettings["FTPUmsPassword"];
        credentials.Url = WebConfigurationManager.AppSettings["FTPUmsUrl"];
        return credentials;
    }
}