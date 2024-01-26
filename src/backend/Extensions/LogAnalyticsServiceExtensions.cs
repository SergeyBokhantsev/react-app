using FunctionApp.Azure;
using FunctionApp.Exceptions;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Extensions
{
    public static class LogAnalyticsServiceExtensions
    {
        private static async Task<T> SearchSingleWithinMultipleWorkspaces<T>(this LogAnalyticsService service, string[] workspaceIds, Func<string, Task<T>> taskGetter)
        {
            async Task<(T? Result, Exception? Exception)> TrySearch(string workspace)
            {
                try
                {
                    var result = await taskGetter(workspace);
                    return (result, null);
                }
                catch (AggregateException ex)
                {
                    return (default, ex.InnerException);
                }
                catch (Exception ex)
                {
                    return (default, ex);
                }
            }

            var tasks = workspaceIds.Select(TrySearch).ToArray();

            var results = await Task.WhenAll(tasks);

            if (results.All(o => null == o.Result))
            {
                if (tasks.Length == 1)
                    throw results.First().Exception!;
                else
                    throw new WorkFlowException(HttpStatusCode.Conflict, string.Join(Environment.NewLine, results.Select(t => t.Exception!.Message)));
            }

            return results.Single(o => o.Result != null).Result!;
        }

        public static async Task<IDictionary<string, object?>> SearchByJobId(this LogAnalyticsService service, string[] workspaceIds, string jobId, DateTimeOffset start, TimeSpan duration)
        {
            return await service.SearchSingleWithinMultipleWorkspaces(workspaceIds,
                                        (workspace) => service.SearchByJobId(workspace, jobId, start, duration));
        }

        public static async Task<IEnumerable<IDictionary<string, object?>>> SearchByEmployeeId(this LogAnalyticsService service, string[] workspaceIds, string employeeId, DateTime from)
        {
            return await service.SearchSingleWithinMultipleWorkspaces(workspaceIds,
                                        (workspace) => service.SearchByEmployeeIdStartingFrom(workspace, employeeId, from, 700, true));
        }
    }
}
