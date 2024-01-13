using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Core;
using Azure.Core.Serialization;
using FunctionApp.Azure;
using FunctionApp.Exceptions;
using FunctionApp.Extensions;
using FunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FunctionApp.Functions
{
    public class SyncJobsFunctions
    {
        private readonly ILogger _logger;
        private readonly ICache cache;
        private readonly SyncDumpService syncDumpService;

        public SyncJobsFunctions(ILoggerFactory loggerFactory, ICache cache, SyncDumpService syncDumpService)
        {
            _logger = loggerFactory.CreateLogger<SyncJobsFunctions>();
            this.cache = cache;
            this.syncDumpService = syncDumpService;
        }

        [Function("get-workspaces")]
        public Task<HttpResponseData> GetWorkspaces([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            return MakeJsonResponse(req, new List<Workspace>()
                {
                    new Workspace { Name="SAPCFC-Internal-QA1", Id = "1eceb865-695e-4711-83b4-de88e6dc504a" },
                    new Workspace { Name="SAPCFC-Internal-Dev1", Id = "3d5fad1f-c86f-432d-9aa1-d55533c283bd" },
                });
        }

        [Function("search-by-job-id")]
        public async Task<HttpResponseData> SearchByJobId([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, FunctionContext context)
        {
            var jobId = req.FromQuery("jobId");
            var workspaceIds = req.FromQuery("workspaceId").Split(':');
            if (!int.TryParse(req.FromQuery("days"), out int days)) throw new ConflictException("Cannot parse provided 'days' parameter");
            if (!int.TryParse(req.FromQuery("related"), out int relatedCount)) throw new ConflictException("Cannot parse provided 'related' parameter");
            if (!workspaceIds.Any())
                throw new WorkFlowException(HttpStatusCode.BadRequest, "No Workspaces selected");

            var service = new LogAnalyticsService(context.GetTokenCredential());

            var result = new List<IDictionary<string, object?>>();

            var targetJob = await cache.GetOrSetAsync(context.GetKeyWithIdentity(jobId), async () =>
            {
                var job = await service.SearchByJobId(workspaceIds, jobId, DateTimeOffset.Now.AddDays(-days), TimeSpan.FromDays(days));
                return (job, TimeSpan.FromDays(3));
            });
                
            targetJob["IsTarget"] = true;

            result.Add(targetJob);

            if (relatedCount > 0)
            {
                var cacheKey = context.GetKeyWithIdentity("relatedJobs", jobId, relatedCount.ToString(), days.ToString());
                var related = await cache.GetOrSetAsync(cacheKey, async () =>
                {
                    List<IDictionary<string, object?>> relatedJobs = new();

                    // predeceasing jobs
                    relatedJobs.AddRange(await service.SearchByEmployeeIdStartingFrom(
                        workspaceId: (string)targetJob["WorkspaceId"]!,
                        employeeId: targetJob.GetProperty<string>("EmployeeId") ?? throw new ConflictException("Job has no Properties.EmployeeId property"),
                        datetime: targetJob.GetEventTime().AddMinutes(-1),
                        count: relatedCount,
                        forward: false));

                    // next jobs
                    relatedJobs.AddRange(await service.SearchByEmployeeIdStartingFrom(
                        workspaceId: (string)targetJob["WorkspaceId"]!,
                        employeeId: targetJob.GetProperty<string>("EmployeeId") ?? throw new ConflictException("Job has no Properties.EmployeeId property"),
                        datetime: targetJob.GetEventTime().AddMinutes(1),
                        count: relatedCount,
                        forward: true));

                    // Do not cache for very long because new related jobs may appear in AI
                    return (relatedJobs, TimeSpan.FromMinutes(1));
                });

                result.AddRange(related);
            }
            
            return await MakeJsonResponse(req, result);
        }

        [Function("get-job-log-by-url")]
        public async Task<HttpResponseData> GetJobLog([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request, FunctionContext context)
        {
            var url = Encoding.UTF8.GetString(Convert.FromBase64String(request.FromQuery("url")));

            var log = await cache.GetOrSetAsync(url, async () => 
            {
                var text = await syncDumpService.GetJobLogAsync(new Uri(url));

                var lines = new List<string>();
                var sb = new StringBuilder();

                foreach (var line in text.Split('\r','\n').Select(x => x.Trim()))
                {
                    if (Regex.IsMatch(line, "^\\d\\d\\d\\d-\\d\\d-\\d\\d") && sb.Length > 0)
                    {
                        lines.Add(sb.ToString());
                        sb.Clear();
                    }

                    sb.AppendLine(line);
                }

                return (lines.ToArray(), TimeSpan.FromDays(7));
            });

            return await MakeJsonResponse(request, log);
        }

        private async Task<HttpResponseData> MakeJsonResponse(HttpRequestData request, object data)
        {
            var response = request.CreateResponse();
            var str = JsonConvert.SerializeObject(data);
            await response.WriteStringAsync(str);
            response.Headers.Add("Content-Type", "application/json");
            return response;
        }
    }
}
