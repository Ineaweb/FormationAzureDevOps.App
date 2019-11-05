using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Collections.Concurrent;

namespace Onepoint.Function
{
    public static class Function01
    {
        private static readonly ConcurrentDictionary<string, IQueueClient> openedQueueClients = new ConcurrentDictionary<string, IQueueClient>();
        private static string endpointUrl = Environment.GetEnvironmentVariable("ServiceBusConnectionString01", EnvironmentVariableTarget.Process);
        private static readonly string serviceBusQueueName = "queue01";

        [FunctionName("Function01")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            log.LogTrace("Set message to send");
            Message message = new Message(Encoding.UTF8.GetBytes(requestBody));

            log.LogDebug("Fetch already open connection in pool");
            IQueueClient queueClient = null;
            if (!openedQueueClients.ContainsKey(serviceBusQueueName))
            {
                log.LogDebug("Create and add a new connection in pool");
                queueClient = openedQueueClients.GetOrAdd(serviceBusQueueName, new QueueClient(endpointUrl, serviceBusQueueName));
                queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
            }
            else
                queueClient = openedQueueClients[serviceBusQueueName];

            log.LogTrace("Transfert message to {QueueName}", serviceBusQueueName);
            await queueClient.SendAsync(message);
            log.LogTrace("Data correctly transfered to {QueueName}", serviceBusQueueName);

            return new AcceptedResult("", new
            {
                Message = "Data correctly forwarded to the service bus"
            });
        }
    }
}
