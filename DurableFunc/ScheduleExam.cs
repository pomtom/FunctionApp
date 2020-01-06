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
        //[FunctionName("ScheduleExam")]
        //public static async Task<List<string>> RunOrchestrator(
        //    [OrchestrationTrigger] IDurableOrchestrationContext context)
        //{
        //    SayHelloRequest data = context.GetInput<SayHelloRequest>();
        //    var outputs = new List<string>();

        //    foreach (var city in data.CityNames)
        //    {
        //        outputs.Add(await context.CallActivityAsync<string>("ScheduleExam_Hello", city));
        //    }

        //    return outputs;
        //}

        //[FunctionName("ScheduleExam_Hello")]
        //public static string SayHello([ActivityTrigger] string name, ILogger log)
        //{
        //    log.LogInformation(name);
        //    return $"AZ-{name}";
        //}


        [FunctionName("Orchestration_ScheduleExam")]
        public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            PartitionKeyGenerator data = context.GetInput<PartitionKeyGenerator>();

            var outputs = new List<string>();
            foreach (var item in data.PartitionKey)
            {
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
                //var data = await req.Content.ReadAsAsync<SayHelloRequest>();

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

    //class SayHelloRequest
    //{
    //    public List<string> CityNames { get; set; }
    //}

    class PartitionKeyGenerator
    {
        public List<string> PartitionKey { get; set; }
    }
}