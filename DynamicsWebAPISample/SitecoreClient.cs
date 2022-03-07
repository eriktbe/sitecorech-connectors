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
            Uri endpoint = new Uri("https://ava-ch06-sandbox06.stylelabs.io/");

            // Enter your credentials here
            OAuthPasswordGrant oauth = new OAuthPasswordGrant
            {
                ClientId = "client_id",
                ClientSecret = "client_secret",
                UserName = "sdkuser",
                Password = "ContentH4b!"
            };

            // Create the Web SDK client
            IWebMClient MClient = MClientFactory.CreateMClient(endpoint, oauth);

            await MClient.TestConnectionAsync();

            //long id = "";
            //string identifier = "r1p8M65j3Euhyo178s9b1A";

            //var loadConfig = new EntityLoadConfiguration(
            //    CultureLoadOption.Default,
            //    new PropertyLoadOption("Title", "FileName", "MainFile", "FileProperties"),
            //    new RelationLoadOption("MasterFile"));

            IEntity entity = await MClient.Entities.GetAsync(assetIdentifier);

            //var filePropsProp = entity.GetProperty<ICultureInsensitiveProperty>("FileProperties");
            //var fileProps = entity.GetPropertyValue<JObject>("FileProperties");
            //var width = fileProps["width"];
            //var height = fileProps["height"];


            //IRelation relation = entity.GetRelation("MasterFile");

            // Get the rendition from the entity
            //var entity = await MConnector.Client.Entities.Get(assetId);
            var rendition = entity.GetRendition("downloadOriginal");
            var renditionItem = rendition?.Items?.FirstOrDefault();

            if (renditionItem == null) return null;

            // Get a stream containing the contents of the rendition
            //using (var stream = await renditionItem.GetStreamAsync())
            //{
            //    //Console.ReadLine();
            //    using (Stream outStream = File.OpenWrite(@"C:\Temp\thorbeckegracht-TEST.jfif"))
            //    {
            //        stream.CopyTo(outStream);

            //    }
            //}

            //return await renditionItem.GetStreamAsync();

            var fileName = entity.GetPropertyValue<string>("FileName");
            var width = entity.GetPropertyValue<long>("Width");
            var height = entity.GetPropertyValue<long>("Height");
            //var masterFileId = entity.GetRelation<IParentToManyChildrenRelation>("MasterFile").Children.FirstOrDefault();

            //if (masterFileId == null) return null;

            //IEntity file = await MClient.Entities.GetAsync(masterFileId);

            //var width = file.GetPropertyValue<int>("Width");
            //var height = file.GetPropertyValue<int>("Height");

            var stream = await renditionItem.GetStreamAsync();
            
            await renditionItem.DownloadAsync(fileName);

            asset.File = stream;
            //asset.Name = "Test Name";
            asset.Name = fileName;
            asset.ContentType = ".png";
            asset.Width = 100;
            asset.Height = 100;

            return asset;
        }
    }

    public class Asset
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Stream File { get; set; }
    }
}
