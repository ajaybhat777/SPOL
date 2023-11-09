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
            context.Load(web, w => w.Navigation.UseShared, w => w.Navigation.QuickLaunch.UseCascading, w => w.Navigation.TopNavigationBar.UseCascading);

            context.ExecuteQuery();

            Console.WriteLine("Navigation Display Style:");

            if (web.Navigation.UseShared)
            {
                Console.WriteLine("Using Shared Navigation");
            }
            else
            {
                if (web.Navigation.QuickLaunch.UseCascading)
                {
                    Console.WriteLine("Using Cascading for Quick Launch");
                }

                if (web.Navigation.TopNavigationBar.UseCascading)
                {
                    Console.WriteLine("Using Cascading for Top Navigation Bar");
                }
            }
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
