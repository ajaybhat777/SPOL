using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Authentication;
using System;
using System.IO;

// Source site information
string sourceSiteUrl = "https://your-source-site-url";
string sourceLibraryName = "Documents";
string sourceFileName = "your-sppkg-file.sppkg";

// Destination site information
string destinationSiteUrl = "https://your-destination-site-url";
string appCatalogSiteUrl = "https://your-app-catalog-site-url";

// Client ID and Client Secret
string clientId = "your-client-id";
string clientSecret = "your-client-secret";

using (ClientContext sourceContext = new AuthenticationManager().GetACSAppOnlyContext(sourceSiteUrl, clientId, clientSecret))
{
    // Load the source file
    Web sourceWeb = sourceContext.Web;
    List sourceLibrary = sourceWeb.Lists.GetByTitle(sourceLibraryName);
    File sourceFile = sourceLibrary.RootFolder.Files.GetByUrl(sourceFileName);
    sourceContext.Load(sourceFile);
    sourceContext.ExecuteQuery();

    // Read the file content from the source
    ClientResult<Stream> fileStream = sourceFile.OpenBinaryStream();
    sourceContext.ExecuteQuery();

    using (ClientContext destinationContext = new AuthenticationManager().GetACSAppOnlyContext(destinationSiteUrl, clientId, clientSecret))
    {
        // Load the app catalog
        var appCatalog = destinationContext.Web.GetAppCatalog(appCatalogSiteUrl);
        destinationContext.Load(appCatalog);
        destinationContext.ExecuteQuery();

        // Upload the file to the app catalog
        var uploadedPackage = appCatalog.UploadDocumentToCatalog(sourceFileName, fileStream.Value, true);
        destinationContext.Load(uploadedPackage);
        destinationContext.ExecuteQuery();

        // Deploy the app package
        var appPackageId = uploadedPackage.UniqueId;
        var deployedPackage = destinationContext.Web.DeployApplicationPackageToAppCatalog(appPackageId);
        destinationContext.Load(deployedPackage);
        destinationContext.ExecuteQuery();

        Console.WriteLine("SPPKG file copied and deployed successfully to the app catalog!");
    }
}
