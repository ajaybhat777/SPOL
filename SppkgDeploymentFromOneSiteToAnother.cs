using Microsoft.Identity.Client;
using PnP.Framework.ALM;
using PnP.Framework.Enums;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using AuthenticationManager = PnP.Framework.AuthenticationManager;

var tenantName = "";
var scopes = new[] { $"https://{tenantName}.sharepoint.com/.default" };
var clientId = "";//id with FullControl
var certificatePassword = "";

var secureString = new SecureString();
foreach (var c in certificatePassword)
{
    secureString.AppendChar(c);
}

var cert = new X509Certificate2(SharePointAppDeployment.Properties.Resources.sharePointOnlineCert1, secureString);

var clientApplication = ConfidentialClientApplicationBuilder
                        .Create(clientId)
                        .WithCertificate(cert)
                        .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantName}.onmicrosoft.com"))
                        .Build();

var result = await clientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
var accessToken = result.AccessToken;

string sourceSiteUrl = $"https://{tenantName}.sharepoint.com/sites/SampleSandbox";
string destinationSiteUrl = $"https://{tenantName}.sharepoint.com/sites/SampleSandboxApps";

var appCatalogName = "Apps For SharePoint";
var sppkgFileName = "sample-app.sppkg";

try
{
    byte[]? sppkgFileBytes = null;
    //------------------------------Copy File-------------------------------------------//
    using (var sourceContext = new AuthenticationManager().GetAccessTokenContext(sourceSiteUrl, accessToken))
    {
        var appCatalogSite = sourceContext.Web;
        var appCatalogLibray = appCatalogSite.Lists.GetByTitle(appCatalogName);
        sourceContext.Load(appCatalogLibray, l => l.RootFolder);
        await sourceContext.ExecuteQueryAsync();

        var sppkgFileUrl = $"{appCatalogLibray.RootFolder.ServerRelativeUrl}/{sppkgFileName}";
        Microsoft.SharePoint.Client.File sppkgFile = sourceContext.Web.GetFileByServerRelativeUrl(sppkgFileUrl);
        sourceContext.Load(sppkgFile);
        await sourceContext.ExecuteQueryAsync();
        Console.WriteLine("Sppkg File Read Successful");

        var stream = sppkgFile.OpenBinaryStream();
        await sourceContext.ExecuteQueryAsync();
        var memStream = new MemoryStream();
        stream.Value.CopyTo(memStream);
        sppkgFileBytes = memStream.ToArray();
    }
    //----------------------------Deploy Package-----------------------------------------------------------------//
    using (var deploymentContext = new AuthenticationManager().GetAccessTokenContext(destinationSiteUrl, accessToken))
    {
        AppManager appManager = new AppManager(deploymentContext);
        var results = appManager.Add(sppkgFileBytes, "sample-app.sppkg", true, AppCatalogScope.Site);
        if(results != null)
        {
            var deployResults = await appManager.DeployAsync(results.Id, true, AppCatalogScope.Site);
            if (deployResults)
            {
                Console.WriteLine("Sppkg Deployment Successful");
            }
            else
            {
                Console.WriteLine("Sppkg Deployment Failed");
            }
        }
        else
        {
            Console.WriteLine("Failed to Add Sppkg File into site AppCatalog");
        }
    }
}
catch (Exception ex) 
{ 
    Console.WriteLine(ex.Message);
}
