using System.Net;
using FunctionApp.Azure;
using FunctionApp.Exceptions;
using FunctionApp.Extensions;
using FunctionApp.Models;
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

        [Function("get-workspaces")]
        public Task<HttpResponseData> GetWorkspaces([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            return MakeResponse(req, new List<Workspace>()
                {
                    new Workspace { Name="SAPCFC-Internal-QA1", Id = "1eceb865-695e-4711-83b4-de88e6dc504a" },
                    new Workspace { Name="SAPCFC-Internal-Dev1", Id = "3d5fad1f-c86f-432d-9aa1-d55533c283bd" },
                });
        }

        [Function("search-by-job-id")]
        public async Task<HttpResponseData> SearchByJobId([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, FunctionContext context)
        {
            var jobId = req.FromQuery("jobId");
            var workspaceId = req.FromQuery("workspaceId");
            if (!int.TryParse(req.FromQuery("days"), out int days)) throw new ConflictException("Cannot parse provided 'days' parameter");
            if (!int.TryParse(req.FromQuery("related"), out int related)) throw new ConflictException("Cannot parse provided 'related' parameter");

            var service = new LogAnalyticsService(context.GetTokenCredential());

            var result = new List<SyncJob>();

            var targetJob = await service.SearchByJobId(workspaceId, jobId, DateTimeOffset.Now.AddDays(-days), TimeSpan.FromDays(days));

            targetJob.IsTarget = true;

            result.Add(targetJob);

            // predeceasing jobs
            result.AddRange(await service.SearchByEmployeeIdStartingFrom(
                workspaceId: workspaceId,
                employeeId: targetJob.EmployeeId,
                datetime: targetJob.Time.AddSeconds(-5),
                count: related,
                forward: false));

            // next jobs
            result.AddRange(await service.SearchByEmployeeIdStartingFrom(
                workspaceId: workspaceId,
                employeeId: targetJob.EmployeeId,
                datetime: targetJob.Time.AddSeconds(5),
                count: related,
                forward: true));

            return await MakeResponse(req, result);
        }

        private async Task<HttpResponseData> MakeResponse(HttpRequestData request, object data)
        {
            var response = request.CreateResponse();
            await response.WriteAsJsonAsync(data);
            response.Headers.Add("Access-Control-Allow-Origin", request.Headers.GetValues("Origin"));
            return response;
        }
    }
}
