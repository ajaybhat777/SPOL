using System;
using Microsoft.SharePoint.Client;

class Program
{
    static void Main()
    {
        string siteUrl = "http://sitecollection/sites/sitename";
        string listTitle = "Site Pages";
        string newWelcomePageUrl = "Forms/AllPages.aspx";

        using (ClientContext context = new ClientContext(siteUrl))
        {
            List list = context.Web.Lists.GetByTitle(listTitle);
            context.Load(list);
            context.ExecuteQuery();

            if (list != null)
            {
                ListItem rootFolder = list.RootFolder;
                rootFolder.Properties["vti_welcomepage"] = newWelcomePageUrl;
                rootFolder.Update();
                context.ExecuteQuery();

                Console.WriteLine("Updated welcome page to: " + newWelcomePageUrl);
            }
            else
            {
                Console.WriteLine("List not found.");
            }
        }
    }
}
