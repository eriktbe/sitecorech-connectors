using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.WebClient;
using Stylelabs.M.Sdk.WebClient.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Framework.Essentials.LoadOptions;
using Newtonsoft.Json.Linq;

namespace DynamicsWebAPISample
{
    public static class SitecoreClient
    {
        public static async Task<Asset> GetAsset(string assetIdentifier)
        {
            var asset = new Asset();

            // Your M endpoint to connect to
            Uri endpoint = new Uri("https://<YOUR CH INSTANCE>.stylelabs.io/");

            // Enter your credentials here
            OAuthPasswordGrant oauth = new OAuthPasswordGrant
            {
                ClientId = "client_id",
                ClientSecret = "client_secret",
                UserName = "sdkuser",
                Password = "<INSERT YOUR PASSWORD HERE>"
            };

            // Create the Web SDK client
            IWebMClient MClient = MClientFactory.CreateMClient(endpoint, oauth);

            IEntity entity = await MClient.Entities.GetAsync(assetIdentifier);

            // Get the rendition from the entity
            var rendition = entity.GetRendition("downloadOriginal");
            var renditionItem = rendition?.Items?.FirstOrDefault();

            if (renditionItem == null) return null;

            var fileName = entity.GetPropertyValue<string>("FileName");
            var width = entity.GetPropertyValue<long>("Width");
            var height = entity.GetPropertyValue<long>("Height");
            
            // Download the asset file so we can retrieve and upload it later
            await renditionItem.DownloadAsync(fileName);

            asset.Name = fileName;
            asset.ContentType = ".png"; // hard-coded for now
            asset.Width = width;
            asset.Height = height;

            return asset;
        }
    }

    public class Asset
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
    }
}
