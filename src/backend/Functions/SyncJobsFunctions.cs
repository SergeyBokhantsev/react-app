using System.Net;
using FunctionApp.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Functions
{
    public class SyncJobsFunctions
    {
        private readonly ILogger _logger;

        public SyncJobsFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SyncJobsFunctions>();
        }

        [Function("search-by-job-id")]
        public HttpResponseData SearchByJobId([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, FunctionContext context)
        {
            var jobId = req.FromQueryRequired("jobId");
            var workspaceId = req.FromQueryRequired("workspaceId");

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
