using System;
using Microsoft.SharePoint.Client;

class Program
{
    static void Main()
    {
        string siteUrl = "your_site_url";
        string userName = "your_username";
        string password = "your_password";

        using (var context = new ClientContext(siteUrl))
        {
            var credentials = new SharePointOnlineCredentials(userName, ConvertToSecureString(password));
            context.Credentials = credentials;

            var web = context.Web;
            context.Load(web, w => w.RootFolder.WelcomePage);
            context.ExecuteQuery();

            Console.WriteLine("Homepage: " + web.RootFolder.WelcomePage);
        }
    }

    static SecureString ConvertToSecureString(string password)
    {
        SecureString secureString = new SecureString();
        foreach (char c in password)
        {
            secureString.AppendChar(c);
        }
        return secureString;
    }
}
