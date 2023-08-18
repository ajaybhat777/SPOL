using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebPartMaintenancePageExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string siteUrl = "http://yoursite";
            string pageUrl = "/pages/yourpage.aspx";

            bool success = await AccessWebPartMaintenancePage(siteUrl, pageUrl);

            if (success)
            {
                Console.WriteLine("Web part maintenance page accessed successfully.");
            }
            else
            {
                Console.WriteLine("Failed to access web part maintenance page.");
            }
        }

        static async Task<bool> AccessWebPartMaintenancePage(string siteUrl, string pageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string maintenanceUrl = $"{siteUrl}/_vti_bin/WebPartPages.asmx";
                    maintenanceUrl += $"/GetWebPartPageDocument?documentPath={pageUrl}&useSource=true";

                    HttpResponseMessage response = await client.GetAsync(maintenanceUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // You can perform further actions based on the response here
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }
    }
}
