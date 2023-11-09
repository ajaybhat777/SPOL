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

            var webFeatures = context.Web.Features;
            context.Load(webFeatures);

            context.ExecuteQuery();

            Console.WriteLine("Active Web Features:");
            foreach (Feature feature in webFeatures)
            {
                Console.WriteLine(feature.DefinitionId);
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
