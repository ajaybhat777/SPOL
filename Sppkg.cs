using OfficeDevPnP.Core.Authentication;
using Microsoft.SharePoint.Client;
using System;

// Source site information
string sourceSiteUrl = "https://your-source-site-url";
string sourceLibraryName = "Documents";
string sourceFileName = "your-sppkg-file.sppkg";

// Destination site information
string destinationSiteUrl = "https://your-destination-site-url";
string destinationLibraryName = "Documents";

// Client ID and Client Secret
string clientId = "your-client-id";
string clientSecret = "your-client-secret";

// Authenticate and load the source file
var authenticationManager = new AuthenticationManager();
using (var sourceContext = authenticationManager.GetACSAppOnlyContext(sourceSiteUrl, clientId, clientSecret))
{
    Web sourceWeb = sourceContext.Web;
    List sourceLibrary = sourceWeb.Lists.GetByTitle(sourceLibraryName);
    File sourceFile = sourceLibrary.RootFolder.Files.GetByUrl(sourceFileName);
    sourceContext.Load(sourceFile);
    sourceContext.ExecuteQuery();

    // Read the file content from the source
    var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(sourceContext, sourceFile.ServerRelativeUrl);
    var fileInfoStream = fileInfo.Stream;

    // Upload the file to the destination library
    using (var destinationContext = authenticationManager.GetACSAppOnlyContext(destinationSiteUrl, clientId, clientSecret))
    {
        Web destinationWeb = destinationContext.Web;
        List destinationLibrary = destinationWeb.Lists.GetByTitle(destinationLibraryName);
        destinationContext.Load(destinationLibrary);
        destinationContext.ExecuteQuery();

        FileCreationInformation fileCreationInfo = new FileCreationInformation();
        fileCreationInfo.ContentStream = fileInfoStream;
        fileCreationInfo.Url = sourceFileName;
        fileCreationInfo.Overwrite = true;

        Microsoft.SharePoint.Client.File newFile = destinationLibrary.RootFolder.Files.Add(fileCreationInfo);
        destinationContext.Load(newFile);
        destinationContext.ExecuteQuery();

        // Deploy the sppkg file
        var solutionId = Guid.NewGuid();
        var app = destinationContext.Web.DeploySolutionAsync(newFile.ServerRelativeUrl, solutionId);
        destinationContext.Load(app);
        destinationContext.ExecuteQuery();

        Console.WriteLine("SPPKG file copied and deployed successfully!");
    }
}
