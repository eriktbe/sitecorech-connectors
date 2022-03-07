using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace DynamicsWebAPISample
{
    public static class DynamicsClient
    {
        public static WhoAmIResponse WhoAmI(HttpClient client)
        {
            WhoAmIResponse returnValue = new WhoAmIResponse();
            //Send the WhoAmI request to the Web API using a GET request. 
            HttpResponseMessage response = client.GetAsync("WhoAmI",
                    HttpCompletionOption.ResponseHeadersRead).Result;
            if (response.IsSuccessStatusCode)
            {
                //Get the response content and parse it.
                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                returnValue.BusinessUnitId = (Guid)body["BusinessUnitId"];
                returnValue.UserId = (Guid)body["UserId"];
                returnValue.OrganizationId = (Guid)body["OrganizationId"];
            }
            else
            {
                throw new Exception(string.Format("The WhoAmI request failed with a status of '{0}'",
                        response.ReasonPhrase));
            }
            return returnValue;
        }

        public static UpsertFileResponse UpsertFile(HttpClient client, Asset asset)
        {
            UpsertFileResponse returnValue = new UpsertFileResponse();

            //HttpStringContent payload = new HttpStringContent();

            //var json = JObject.Parse()

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

            var request = new UpsertFileRequest()
            {
                //InputFile = $"{{\"msdyncrm_name\": \"{timestamp}.png\",\"msdyncrm_contenttype\" : \".png\",\"msdyncrm_width\": 100,\"msdyncrm_height\": 100}}",
                InputFile = $"{{\"msdyncrm_name\": \"{asset.Name}\",\"msdyncrm_contenttype\" : \"{asset.ContentType}\",\"msdyncrm_width\": {asset.Width},\"msdyncrm_height\": {asset.Height}}}",
                InputKeywords = "{\"id\": \"5eca04ef-b582-ec11-8d20-6045bd8dbb00\",\"entityType\": \"msdyncrm_keyword\",\"name\": \"test\"}"
            };

            JObject json = (JObject)JToken.FromObject(request);

            StringContent httpContent = new StringContent(json.ToString(), System.Text.Encoding.UTF8, "application/json");


            //Send the UpsertFile request to the Web API using a POST request. 
            HttpResponseMessage response = client.PostAsync("msdyncrm_UpsertFile", 
                    httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                //Get the response content and parse it.
                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                //returnValue.OutputFile = (string)body["OutputFile"];

                string outputFileString = (string)body["OutputFile"];

                JObject outputFile = JObject.Parse(outputFileString);

                returnValue.OutputFile.BlobUri = (string)outputFile["msdyncrm_bloburi"];
                returnValue.OutputFile.FileId = (string)outputFile["msdyncrm_fileid"];
                returnValue.OutputFile.SasToken = (string)outputFile["msdyncrm_sastoken"];

            }
            else
            {
                throw new Exception(string.Format("The UpsertFile request failed with a status of '{0}'",
                        response.ReasonPhrase));
            }
            return returnValue;
        }

        public static void UpdateFileEntity(HttpClient client, string fileId)
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

            StringContent httpContent = new StringContent("{\"msdyncrm_rethumbnail\": \"true\"}", System.Text.Encoding.UTF8, "application/json");


            //Send the UpsertFile request to the Web API using a POST request. 
            HttpResponseMessage response = client.PatchAsync($"msdyncrm_files({fileId})",
                    httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                ////Get the response content and parse it.
                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                ////returnValue.OutputFile = (string)body["OutputFile"];

                //string outputFileString = (string)body["OutputFile"];

                //JObject outputFile = JObject.Parse(outputFileString);

                //returnValue.OutputFile.BlobUri = (string)outputFile["msdyncrm_bloburi"];
                //returnValue.OutputFile.FileId = (string)outputFile["msdyncrm_fileid"];
                //returnValue.OutputFile.SasToken = (string)outputFile["msdyncrm_sastoken"];

            }
            else
            {
                throw new Exception(string.Format("The UpdateFileEntity request failed with a status of '{0}'",
                        response.ReasonPhrase));
            }
            //return returnValue;
        }
    }

    public class UpsertFileRequest
    {
        public string InputFile { get; set; }
        public string InputKeywords { get; set; }
    }

    public class UpsertFileResponse
    {
        //public string OutputFile { get; set; }

        public OutputFile OutputFile { get; set; } = new OutputFile();
    }

    public class OutputFile
    {
        public string BlobUri { get; set; }
        public string FileId { get; set; }
        public string SasToken { get; set; }
    }

    //public class UpdateFileEntityRequest
    //{
    //    public bool
    //}

    //public class UpdateFileEntityResponse
    //{

    //}
}
