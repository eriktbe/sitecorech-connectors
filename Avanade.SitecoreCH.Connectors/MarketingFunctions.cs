using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DynamicsWebAPISample;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Avanade.SitecoreCH.Connectors
{
    public class MarketingFunctions
    {
        [FunctionName("AssetApprovedFunction")]
        public void Run([ServiceBusTrigger("sitecorech-events", Connection = "SitecoreCHServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            JObject body = JObject.Parse(myQueueItem);
            var message = body["saveEntityMessage"];
            var assetIdentifier = (string)message["TargetIdentifier"];

            try
            {
                // Get the asset file + details from Sitecore CH
                var asset = SitecoreClient.GetAsset(assetIdentifier).Result;

                string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:Connect");

                using (HttpClient client = DynamicsHelpers.GetHttpClient(connectionString, DynamicsHelpers.clientId, DynamicsHelpers.redirectUrl))
                {
                    // Call the UpsertFile method to create file entity in D365 Marketing
                    UpsertFileResponse response2 = DynamicsClient.UpsertFile(client, asset);
                    log.LogInformation($"Your blob URI is: {response2.OutputFile.BlobUri}");
                    log.LogInformation($"Your file ID is: {response2.OutputFile.FileId}");
                    log.LogInformation($"Your SAS token is: {response2.OutputFile.SasToken}");

                    // Upload file to Azure blob storage
                    AzureBlobClient.UploadFile(response2.OutputFile.BlobUri, response2.OutputFile.SasToken, asset);

                    log.LogInformation($"File uploaded successfully");

                    // Call the Files method to generate thumbnails in D365 Marketing
                    DynamicsClient.UpdateFileEntity(client, response2.OutputFile.FileId);

                    log.LogInformation($"File entity updated successfully");
                }
            }
            catch (Exception ex)
            {
                DynamicsHelpers.DisplayException(ex, log);
            }
        }
    }
}
