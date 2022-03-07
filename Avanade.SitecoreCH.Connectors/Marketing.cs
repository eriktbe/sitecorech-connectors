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
    //public class SaveEntityMessage
    //{
    //    public string EventType { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //    public bool IsNew { get; set; }
    //    public string TargetDefinition { get; set; }
    //    public long TargetId { get; set; }
    //    public string TargetIdentifier { get; set; }
    //    public DateTime CreatedOn { get; set; }
    //    public long UserId { get; set; }
    //    public int Version { get; set; }
    //}

    //public class SaveEntityMessageWrapper
    //{
    //    public SaveEntityMessage saveEntityMessage { get; set; }
    //}

    //"{\"saveEntityMessage\":{\"EventType\":\"EntityCreated\",\"TimeStamp\":\"2022-03-05T15:17:34.682Z\",\"IsNew\":true,\"TargetDefinition\":\"M.Asset\",\"TargetId\":31410,\"TargetIdentifier\":\"In8_ntSDxka_mMzp5yCEOw\",\"CreatedOn\":\"2022-03-05T15:17:34.6812201Z\",\"UserId\":31322,\"Version\":1,\"ChangeSet\":{\"PropertyChanges\":[{\"Culture\":\"(Default)\",\"Property\":\"FileName\",\"Type\":\"System.String\",\"OriginalValue\":null,\"NewValue\":\"IMG_9818_edited.jpg\"},{\"Culture\":\"(Default)\",\"Property\":\"Title\",\"Type\":\"System.String\",\"OriginalValue\":null,\"NewValue\":\"IMG_9818_edited.jpg\"},{\"Culture\":\"(Default)\",\"Property\":\"Asset.ExplicitApprovalRequired\",\"Type\":\"System.Boolean\",\"OriginalValue\":null,\"NewValue\":false},{\"Culture\":\"(Default)\",\"Property\":\"PublishStatus\",\"Type\":\"System.String\",\"OriginalValue\":null,\"NewValue\":\"UpdatingPublishStatus\"}],\"Cultures\":[\"(Default)\"],\"RelationChanges\":[{\"Relation\":\"FinalLifeCycleStatusToAsset\",\"Role\":1,\"Cardinality\":0,\"NewValues\":[542],\"RemovedValues\":[],\"inherits_security_original\":null,\"inherits_security\":true},{\"Relation\":\"ContentRepositoryToAsset\",\"Role\":1,\"Cardinality\":1,\"NewValues\":[734],\"RemovedValues\":[],\"inherits_security_original\":null,\"inherits_security\":true},{\"Relation\":\"DRM.Restricted.RestrictedToAsset\",\"Role\":1,\"Cardinality\":0,\"NewValues\":[886],\"RemovedValues\":[],\"inherits_security_original\":null,\"inherits_security\":true}],\"inherits_security_original\":null,\"inherits_security\":true,\"is_root_taxonomy_item_original\":null,\"is_root_taxonomy_item\":false,\"is_path_root_original\":null,\"is_path_root\":false,\"is_system_owned_original\":null,\"is_system_owned\":false}},\"context\":{}}"


    public class Marketing
    {
        [FunctionName("AssetApprovedFunction")]
        public void Run([ServiceBusTrigger("inbound-sitecorech", Connection = "SitecoreCHServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            //var message = JsonSerializer.Deserialize<SaveEntityMessageWrapper>(myQueueItem)?.saveEntityMessage;

            JObject body = JObject.Parse(myQueueItem);
            var message = body["saveEntityMessage"];
            var assetIdentifier = (string)message["TargetIdentifier"];

            try
            {
                //Get configuration data from App.config connectionStrings
                //string connectionString = Configuration.GetConnectionString();
                //test 123

                var asset = SitecoreClient.GetAsset(assetIdentifier).Result;

                string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:Connect");

                using (HttpClient client = SampleHelpers.GetHttpClient(connectionString, SampleHelpers.clientId, SampleHelpers.redirectUrl))
                {
                    // Use the WhoAmI function
                    WhoAmIResponse response = DynamicsClient.WhoAmI(client);
                    log.LogInformation($"Your system user ID is: {response.UserId}");
                    log.LogInformation($"Your system business unit ID is: {response.BusinessUnitId}");
                    log.LogInformation($"Your system organization ID is: {response.OrganizationId}");

                    // Use the UpsertFile function
                    UpsertFileResponse response2 = DynamicsClient.UpsertFile(client, asset);
                    log.LogInformation($"Your blob URI is: {response2.OutputFile.BlobUri}");
                    log.LogInformation($"Your file ID is: {response2.OutputFile.FileId}");
                    log.LogInformation($"Your SAS token is: {response2.OutputFile.SasToken}");

                    AzureBlobClient.UploadFile(response2.OutputFile.BlobUri, response2.OutputFile.SasToken, asset);

                    log.LogInformation($"File uploaded successfully");

                    DynamicsClient.UpdateFileEntity(client, response2.OutputFile.FileId);

                    log.LogInformation($"File entity updated successfully");

                    //log.LogInformation($"Press any key to exit.");
                    //Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.DisplayException(ex, log);
                //Console.WriteLine("Press any key to exit.");
                //Console.ReadLine();
            }
        }
    }
}
