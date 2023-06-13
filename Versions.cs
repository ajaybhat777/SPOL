using System;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        string siteUrl = "http://your-site-url"; // Replace with your SharePoint site URL
        string listId = "your-list-guid"; // Replace with the List ID
        int itemId = 1; // Replace with the Item ID
        int minorVersion = 1; // Replace with the minor version number
        int majorVersion = 2; // Replace with the major version number

        string endpointUrl = $"{siteUrl}/_api/web/lists(guid'{listId}')/items({itemId})/versions(majorVersion={majorVersion},minorVersion={minorVersion})";

        // Create the HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpointUrl);
        request.Method = "GET";
        request.Accept = "application/json;odata=verbose";

        // Send the request and get the response
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseText = reader.ReadToEnd();
                    JObject data = JObject.Parse(responseText);

                    // Extract the desired information from the response
                    string title = (string)data["d"]["Title"];
                    string description = (string)data["d"]["Description"];
                    // Retrieve other properties as needed

                    // Display the retrieved information
                    Console.WriteLine("Title: " + title);
                    Console.WriteLine("Description: " + description);
                    // Display other properties as needed
                }
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }
        }

        Console.ReadLine();
    }
}
