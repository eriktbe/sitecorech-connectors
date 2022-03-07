using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp2
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("myqueue", Connection = "test2connstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
