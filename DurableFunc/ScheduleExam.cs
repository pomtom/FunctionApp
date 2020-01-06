using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DurableFunc
{
    public static class ScheduleExam
    {

        [FunctionName("Orchestration_ScheduleExam")]
        public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {

            PartitionKeyGenerator data = context.GetInput<PartitionKeyGenerator>();

            string tableName = Constants.TableName;
            TableStorage tableStore = new TableStorage(tableName);

            var outputs = new List<string>();
            foreach (var item in data.PartitionKey)
            {
                List<CandidatesEntity> candidates = tableStore.GetAll<CandidatesEntity>(item);
                outputs.Add(await context.CallActivityAsync<string>("Activity_ScheduleExam", item));
            }
            return outputs;
        }

        [FunctionName("Activity_ScheduleExam")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation(name);
            return $"AZ-{name}";
        }

        [FunctionName("ScheduleExam_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            try
            {
                // INPUT : {"PartitionKey":["input-040120201213","input-050120201213","input-060120201213"]}
                var data = await req.Content.ReadAsAsync<PartitionKeyGenerator>();

                // Function input comes from the request content.
                string instanceId = await starter.StartNewAsync("Orchestration_ScheduleExam", data);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }

    class PartitionKeyGenerator
    {
        public List<string> PartitionKey { get; set; }
    }
}