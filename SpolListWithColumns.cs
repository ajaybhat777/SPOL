using System;
using System.Security;
using Microsoft.SharePoint.Client;

class Program
{
    static void Main(string[] args)
    {
        string siteUrl = "https://your-sharepoint-online-site.sharepoint.com/sites/yoursite";
        string username = "your-username@yourdomain.com";
        string password = "your-password";

        var securePassword = new SecureString();
        foreach (char c in password.ToCharArray())
        {
            securePassword.AppendChar(c);
        }

        using (var context = new ClientContext(siteUrl))
        {
            context.Credentials = new SharePointOnlineCredentials(username, securePassword);

            Web web = context.Web;
            ListCollection lists = web.Lists;

            context.Load(web);
            context.Load(lists);
            context.ExecuteQuery();

            Console.WriteLine("Custom Columns in the SharePoint Online site:");

            foreach (List list in lists)
            {
                FieldCollection fields = list.Fields;
                context.Load(fields);
                context.ExecuteQuery();

                Console.WriteLine($"List: {list.Title}");
                foreach (Field field in fields)
                {
                    if (!field.ReadOnlyField)
                    {
                        Console.WriteLine($"- {field.Title}");
                    }
                }
            }
        }
    }
}
