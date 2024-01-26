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

        public static string? FromQueryOptional(this HttpRequestData requestData, string name)
        {
            return requestData.Query.Get(name);                   
        }

        public static string FromQuery(this HttpRequestData requestData, string name)
        {
            return requestData.FromQueryOptional(name)
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

        public static Uri GetSyncDumpDownloadUrl(this IDictionary<string, object?> customEvent)
        {
            var url = customEvent.GetProperty<string>("JobLogUrl") ?? throw new ConflictException("No JobLogUrl field");
            return new Uri(url);
        }
    }
}
