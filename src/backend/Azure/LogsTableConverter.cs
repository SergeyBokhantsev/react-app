using Azure.Monitor.Query.Models;
using FunctionApp.Exceptions;
using FunctionApp.Models;
using Newtonsoft.Json;

namespace FunctionApp.Azure
{
    public static class LogsTableConverter
    {
        public static IEnumerable<Dictionary<string, object?>> ToDictionary(this LogsTable table, string workspaceId)
        {
            foreach (var row in table.Rows)
            {
                var result = new Dictionary<string, object?>();
                result["WorkspaceId"] = workspaceId;

                for (int i = 0; i < table.Columns.Count; ++i)
                {
                    switch (table.Columns[i].Type.ToString())
                    {
                        case "dynamic":
                            result[table.Columns[i].Name] = 
                                JsonSerializer.CreateDefault()
                                .Deserialize(new JsonTextReader(new StringReader(row.GetString(i))));
                            break;

                        case "datetime":
                            result[table.Columns[i].Name] = row.GetDateTimeOffset(i)!.Value.UtcDateTime;
                            break;

                        default:
                            result[table.Columns[i].Name] = row[i];
                            break;
                    }
                }

                yield return result;
            }
        }
    }
}
