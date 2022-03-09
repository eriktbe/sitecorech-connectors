using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace DynamicsWebAPISample
{
    public static class AzureBlobClient
    {
        public static void UploadFile(string blobUri, string sasToken, Asset asset)
        {
            // Retrieve our downloaded asset file
            using var stream = System.IO.File.OpenRead(asset.Name);

            StreamContent httpContent = new StreamContent(stream);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");

            // Upload the file to Azure blob storage using SAS token
            HttpResponseMessage response = client.PutAsync($"{blobUri}{sasToken}",
                    httpContent).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("The UploadFile request failed with a status of '{0}'",
                        response.ReasonPhrase));
            }
        }
    }
}
