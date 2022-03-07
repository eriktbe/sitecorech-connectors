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
            //UpsertFileResponse returnValue = new UpsertFileResponse();

            //HttpStringContent payload = new HttpStringContent();

            //var json = JObject.Parse()

            //var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

            //var request = new UpsertFileRequest()
            //{
            //    InputFile = $"{{\"msdyncrm_name\": \"{timestamp}.png\",\"msdyncrm_contenttype\" : \".png\",\"msdyncrm_width\": 100,\"msdyncrm_height\": 100}}",
            //    InputKeywords = "{\"id\": \"5eca04ef-b582-ec11-8d20-6045bd8dbb00\",\"entityType\": \"msdyncrm_keyword\",\"name\": \"test\"}"
            //};

            //JObject json = (JObject)JToken.FromObject(request);

            //StringContent httpContent = new StringContent(json.ToString(), System.Text.Encoding.UTF8, "application/json");

            //using var stream = System.IO.File.OpenRead("./img/example.png");
            using var stream = System.IO.File.OpenRead(asset.Name);


            //long contentLength = file.Length;


            //StreamContent httpContent = new StreamContent(asset.File);
            StreamContent httpContent = new StreamContent(stream);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");

            //Send the UpsertFile request to the Web API using a POST request. 
            HttpResponseMessage response = client.PutAsync($"{blobUri}{sasToken}",
                    httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                //Get the response content and parse it.
                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                //returnValue.OutputFile = (string)body["OutputFile"];

            }
            else
            {
                throw new Exception(string.Format("The UploadFile request failed with a status of '{0}'",
                        response.ReasonPhrase));
            }
            //return returnValue;
        }
    }
}
