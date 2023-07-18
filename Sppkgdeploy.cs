using Microsoft.SharePoint.Client;
using System.IO;

public void DeployAppPackage(string destinationSiteUrl, string sppkgFilePath)
{
    // Connect to the destination site
    using (var context = new ClientContext(destinationSiteUrl))
    {
        // Retrieve the App Catalog
        var appCatalog = context.Site.GetCatalog((int)ListTemplateType.AppCatalog);
        context.Load(appCatalog);
        context.ExecuteQuery();

        // Upload and deploy the app package to the App Catalog
        var fileStream = new FileStream(sppkgFilePath, FileMode.Open);
        var fileInfo = new FileInfo(sppkgFilePath);

        var fileCreationInfo = new FileCreationInformation
        {
            ContentStream = fileStream,
            Url = fileInfo.Name,
            Overwrite = true
        };

        var appPackage = appCatalog.RootFolder.Files.Add(fileCreationInfo);
        context.Load(appPackage);
        context.ExecuteQuery();

        // Deploy the app package
        appPackage.DeployAppPackage(true);

        context.ExecuteQuery();
    }
}
