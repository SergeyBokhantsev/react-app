using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using FunctionApp.Exceptions;
using FunctionApp.Models;
using System.Net;

namespace FunctionApp.Azure
{
    public class LogAnalyticsService
    {
        private readonly LogsQueryClient client;

        public LogAnalyticsService(TokenCredential credential)
        {
            client = new LogsQueryClient(credential);
        }

        public async Task<IEnumerable<SyncJob>> SearchByEmployeeIdStartingFrom(string workspaceId, object employeeId, DateTime datetime, int count, bool forward)
        {
            var query = $"AppEvents | where Name startswith 'Jobs/' and tostring(Properties.EmployeeId) == '{employeeId}' and TimeGenerated {(forward ? ">" : "<")} todatetime('{datetime.ToString("O")}Z') | take {count}";

            var result = await client.QueryWorkspaceAsync(workspaceId, query, QueryTimeRange.All);

            if (result.Value.Status != LogsQueryResultStatus.Success)
                throw new ConflictException(result.Value.Error.ToString());

            return result.Value.Table.ToSyncJobInfoes(workspaceId);
        }

        public async Task<SyncJob> SearchByJobId(string workspaceId, string jobId, DateTimeOffset start, TimeSpan duration)
        {
            var query = $"AppEvents | where Name startswith 'Jobs/' and tostring(Properties.JobId) == '{jobId}'";

            var result = await client.QueryWorkspaceAsync(workspaceId, query, new QueryTimeRange(start, duration));

            if (result.Value.Status != LogsQueryResultStatus.Success)
                throw new ConflictException(result.Value.Error.ToString());

            if (result.Value.Table.Rows.Count != 1)
                throw new WorkFlowException(HttpStatusCode.NotFound, $"Job {jobId} is not found");

            return result.Value.Table.ToSyncJobInfoes(workspaceId).Single();
        }
    }
}
