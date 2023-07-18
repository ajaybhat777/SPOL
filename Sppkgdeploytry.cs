using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PnP.Framework;
using PnP.Framework.Authentication;
using PnP.Framework.Provisioning.Connectors;

class Program
{
    static void Main(string[] args)
    {
        // Load configuration settings
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        // Retrieve configuration settings
        string clientId = configuration["ClientId"];
        string tenantId = configuration["TenantId"];
        string clientCertificatePath = configuration["ClientCertificatePath"];
        string clientCertificatePassword = configuration["ClientCertificatePassword"];

        // Source site details
        string sourceSiteUrl = "https://source-site-url";
        string sourceSiteRelativePath = "/sites/source-site";
        string sourceSppkgFileName = "YourPackage.sppkg";

        // Destination site details
        string destinationSiteUrl = "https://destination-site-url";
        string destinationSiteRelativePath = "/sites/destination-site";

        // Create authentication manager
        X509Certificate2 clientCertificate = new X509Certificate2(clientCertificatePath, clientCertificatePassword);
        CertificateManager certManager = new CertificateManager(clientCertificate);
        AuthenticationManager authManager = new AuthenticationManager(clientId, tenantId, certManager);

        // Connect to source site
        using (var sourceContext = authManager.GetContext(sourceSiteUrl))
        {
            // Get the sppkg file from the source site's App Catalog
            var sppkgFile = sourceContext.Web.GetFileByServerRelativeUrl(sourceSiteRelativePath + "/AppCatalog/" + sourceSppkgFileName);
            sourceContext.Load(sppkgFile);
            sourceContext.ExecuteQueryRetry();

            // Connect to destination site
            using (var destinationContext = authManager.GetContext(destinationSiteUrl))
            {
                // Upload the sppkg file to the destination site's App Catalog
                var appCatalog = destinationContext.Web.GetListByUrl(destinationSiteRelativePath + "/AppCatalog");
                var fileInfo = new FileConnectorFile(sppkgFile.ServerRelativeUrl, sppkgFile.OpenBinaryStream());
                appCatalog.RootFolder.UploadFile(sourceSppkgFileName, fileInfo, true);
                destinationContext.ExecuteQueryRetry();

                // Deploy the uploaded sppkg file
                appCatalog.RootFolder.DeployApplicationPackageToAppCatalog(sourceSppkgFileName, true);
                destinationContext.ExecuteQueryRetry();

                Console.WriteLine("Package deployed successfully.");
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
