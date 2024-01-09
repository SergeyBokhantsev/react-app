using Azure.Core;
using FunctionApp.Azure;
using FunctionApp.Exceptions;
using FunctionApp.Middleware;
using FunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Extensions
{
    public static class Extensions
    {
        public static string FromQuery(this HttpRequestData requestData, string name)
        {
            return requestData.Query.Get(name)
                   ?? throw new WorkFlowException(HttpStatusCode.BadRequest, $"Required query parameter is not exists: {name}");
        }

        public static TokenCredential GetTokenCredential(this FunctionContext context)
        {
            return context.Items[nameof(TokenExtractionMiddleware)] as TokenCredential
                ?? throw new WorkFlowException(HttpStatusCode.Unauthorized, "This method requires for an authorization token");
        }

        public static async Task<SyncJob> SearchByJobId(this LogAnalyticsService service, string[] workspaceIds, string jobId, DateTimeOffset start, TimeSpan duration)
        {
            async Task<(SyncJob? Job, Exception? Exception)> TrySearch(string workspace, string jobId, DateTimeOffset start, TimeSpan duration)
            {
                try
                {
                    var job = await service.SearchByJobId(workspace, jobId, start, duration);
                    return (job, null);
                }
                catch (AggregateException ex)
                {
                    return (null, ex.InnerException);
                }
                catch (Exception ex)
                {
                    return (null, ex);
                }
            }

            var tasks = workspaceIds.Select(ws => TrySearch(ws, jobId, start, duration)).ToArray();

            var results = await Task.WhenAll(tasks);

            if (results.All(o => null == o.Job))
            {
                if (tasks.Length == 1)
                    throw results.First().Exception!;
                else
                    throw new WorkFlowException(HttpStatusCode.Conflict, string.Join(Environment.NewLine, results.Select(t => t.Exception!.Message)));
            }

            return results.First(o => o.Job != null).Job!;
        }
    }
}
