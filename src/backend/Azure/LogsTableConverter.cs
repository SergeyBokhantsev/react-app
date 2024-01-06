using Azure.Monitor.Query.Models;
using FunctionApp.Exceptions;
using FunctionApp.Models;
using System.Text.Json;

namespace FunctionApp.Azure
{
    public static class LogsTableConverter
    {
        public static IEnumerable<SyncJob> ToSyncJobInfoes(this LogsTable table)
        {
            foreach (var row in table.Rows)
            {
                var props = row.GetAndDeserialize<SyncJobProperties>("Properties")
                            ?? throw new ConflictException("Cannot parse the Properties column");

                if (string.IsNullOrWhiteSpace(props.JobId))
                    throw new ConflictException("JobId is not found in the Properties column");

                if (string.IsNullOrWhiteSpace(props.EmployeeId))
                    throw new ConflictException("EmployeeId is not found in the Properties column");

                var name = row.GetString("Name");
                var status = name.Substring("Jobs/".Length);

                var job = new SyncJob
                {
                    Id = props.JobId,
                    EmployeeId = props.EmployeeId,
                    Status = status,
                    Properties = props
                };
                  
                job.AppRoleInstance = row.GetString("AppRoleInstance");
                job.Time = (row.GetDateTimeOffset("TimeGenerated") ?? throw new ConflictException("Cannot parse the TimeGenerated column")).DateTime;
                job.OperationName = row.GetString("OperationName");
                job.AppVersion = row.GetString("AppVersion");

                yield return job;
            }
        }

        private static T? GetAndDeserialize<T>(this LogsTableRow row, string name)
        {
            var str = row.GetString(name);
            return JsonSerializer.Deserialize<T?>(str);
        }
    }
}
