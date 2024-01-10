using Azure.Core;
using Azure.Core.Serialization;
using FunctionApp.Azure;
using FunctionApp.Exceptions;
using FunctionApp.Middleware;
using FunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
        public static IFunctionsWorkerApplicationBuilder UseNewtonsoftJson(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.Services.Configure<WorkerOptions>(workerOptions =>
            {
                var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                settings.NullValueHandling = NullValueHandling.Ignore;
                workerOptions.Serializer = new NewtonsoftJsonObjectSerializer(settings);
            });

            return builder;
        }

        public static string FromQuery(this HttpRequestData requestData, string name)
        {
            

            return requestData.Query.Get(name)
                   ?? throw new WorkFlowException(HttpStatusCode.BadRequest, $"Required query parameter is not exists: {name}");
        }

        public static TokenCredential GetTokenCredential(this FunctionContext context)
        {
            return context.Items[TokenExtractionMiddleware.AzureCredential] as TokenCredential
                ?? throw new WorkFlowException(HttpStatusCode.Unauthorized, "This method requires for an authorization token");
        }

        public static string GetKeyWithIdentity(this FunctionContext context, params string[] keys)
        {
            var identityHash = context.Items[TokenExtractionMiddleware.IdentityHash] as string
                ?? throw new WorkFlowException(HttpStatusCode.Conflict, "Identity hash is not exists");

            return string.Concat(string.Join(':', keys), identityHash);
        }

        public static string GetEmployeeId(this IDictionary<string, object?> syncJob)
        {
            if (!syncJob.TryGetValue("Properties", out var props))
                throw new ConflictException("Cannot get EmployeeId: Properties field is not exists in the syncJob");

            if (null == props)
                throw new ConflictException("Cannot get EmployeeId: Properties field is null");

            if (!(props is JObject jObj))
                throw new ConflictException($"Cannot get EmployeeId: Properties field is of unexpected type {props.GetType()}");

            throw new Exception();
        }

        public static async Task<IDictionary<string, object?>> SearchByJobId(this LogAnalyticsService service, string[] workspaceIds, string jobId, DateTimeOffset start, TimeSpan duration)
        {
            async Task<(IDictionary<string, object?>? Job, Exception? Exception)> TrySearch(string workspace, string jobId, DateTimeOffset start, TimeSpan duration)
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

        public static T? GetProperty<T>(this IDictionary<string, object?> customEvent, string name)
        {
            var jObj = customEvent["Properties"] as JObject ?? throw new ConflictException("Properties is not of JObject type or null");
            var jToken = jObj[name] ?? throw new ConflictException($"The Properties has no {name} field");
            return jToken.Value<T>();
        }

        public static DateTime GetEventTime(this IDictionary<string, object?> customEvent)
        {
            return (DateTime)customEvent["TimeGenerated"]!;
        }
    }
}
